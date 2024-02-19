using Game;
using Moq;
using NUnit.Framework;
using UserCommunication;

#nullable disable

namespace DiceGameTests;

public class GuessingGameTests
{
    private Mock<IDice> _diceMock;
    private Mock<IConsoleInteraction> _consoleInteractionMock;
    private GuessingGame _cut;


    [SetUp]
    public void Setup()
    {
        _diceMock = new();
        _consoleInteractionMock = new();
        _cut = new(_diceMock.Object, _consoleInteractionMock.Object);
    }

    [TestCase(3)]
    [TestCase(1)]
    [TestCase(5)]
    public void Play_ShouldReturnVictory_OnFirstGuessBeingCorrect(int numberOnDie)
    {
        _diceMock
            .Setup(mock => mock.Roll())
            .Returns(numberOnDie);
        _consoleInteractionMock
            .Setup(mock => mock.ReadInteger(It.IsAny<string>()))
            .Returns(numberOnDie);

        var gameResult = _cut.Play();

        _consoleInteractionMock.Verify(mock => mock.ShowMessage(ResourceThing.WrongNumber), Times.Never());

        Assert.That(gameResult, Is.EqualTo(GameResult.Victory));
    }

    [TestCase(3, 4)]
    [TestCase(1, 3)]
    [TestCase(5, 6)]
    public void Play_ShouldReturnLoss_WhenUserIsOutOfTries(int numberOnDie, int wrongGuess)
    {
        _diceMock
            .Setup(mock => mock.Roll())
            .Returns(numberOnDie);

        _consoleInteractionMock
            .Setup(mock => mock.ReadInteger(It.IsAny<string>()))
            .Returns(wrongGuess);

        var gameResult = _cut.Play();

        _consoleInteractionMock.Verify(mock => mock.ShowMessage(It.IsAny<string>()), Times.Exactly(3));

        Assert.That(gameResult, Is.EqualTo(GameResult.Loss));
    }

    [TestCase(3)]
    [TestCase(1)]
    [TestCase(5)]
    public void Play_ShouldReturnVictory_IfUserGuessedCorrectlyButNotOnFirstTry(int numberOnDie)
    {
        _diceMock
            .Setup(mock => mock.Roll())
            .Returns(numberOnDie);
        _consoleInteractionMock
            .SetupSequence(mock => mock.ReadInteger(It.IsAny<string>()))
            .Returns(numberOnDie + 1)
            .Returns(numberOnDie + 1)
            .Returns(numberOnDie);

        var gameResult = _cut.Play();

        _consoleInteractionMock.Verify(mock => mock.ShowMessage(It.IsAny<string>()), Times.AtLeast(1));

        Assert.That(gameResult, Is.EqualTo(GameResult.Victory));
    }


    [Test]
    public void PrintResult_PrintWinningMessage_IfGameResultIsVictory()
    {
        _cut.PrintResult(GameResult.Victory);

        _consoleInteractionMock.Verify(mock => mock.ShowMessage("You win!"));
    }

    [Test]
    public void PrintResult_PrintLosingMessage_IfGameResultIsLoss()
    {
        _cut.PrintResult(GameResult.Loss);

        _consoleInteractionMock.Verify(mock => mock.ShowMessage("You lose :("));
    }
}
