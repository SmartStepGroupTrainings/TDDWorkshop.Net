﻿using System;
using System.IO;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PlayerTests : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            Create = new Father();
        }
        [Test]
        public void EnterGame_Player_InGame()
        {
            var player = CreatePlayer();
            var game = CreateGame();

            player.Enter(game);

            Assert.IsTrue(player.IsIn(game));
        }

        [Test]
        public void ByDefaultExitGame_Player_NotInGame()
        {
            var player = CreatePlayer();
            var game = CreateGame();
            player.Enter(game);
            player.Exit();

            var e = Assert.Throws<InvalidOperationException>(player.Exit);
            Assert.AreEqual("Не войдя не можешь ты выйти, юный падован", e.Message);
        }

        [Test]
        public void ExitGame_Player_NotInGame()
        {
            var player = CreatePlayer();
            var game = CreateGame();

            player.Enter(game);
            player.Exit();

            Assert.IsFalse(player.IsIn(game));
        }

        [Test]
        public void JoinGameWhereHasPlayer_Player_InGame()
        {
            var player = CreatePlayer();
            var anotherPlayer = CreatePlayer();
            var game = CreateGame();

            player.Enter(game);
            anotherPlayer.Enter(game);

            Assert.IsTrue(anotherPlayer.IsIn(game));
        }

        [Test]
        public void OneGame_Player_InGame()
        {
            var player = CreatePlayer();
            var game = CreateGame();

            player.Enter(game);

            var e = Assert.Throws<InvalidOperationException>(() => player.Enter(game));
            Assert.AreEqual("Можешь играть в одну игру ты только, юный падован", e.Message);
        }

        [Test]
        public void BuyChips_Player_CanDoBets()
        {
            var player = new Player();

            player.BuyChips(1);

            Assert.IsTrue(player.CanDoBets());
        }

        [Test]
        public void DoBet_Player_MayWin()
        {
            Game game = Create.Game;
            Player player = Create.Player.In(game);

            player.DoBet(CreateBet());

            Assert.IsTrue(player.HasBets());
        }

        [Test]
        public void CanBeInGame_Player_DoBet()
        {
            var player = new Player();
            var game = new Game();
            player.Enter(game);
            player.Exit();

            Assert.Throws<InvalidOperationException>(() => player.DoBet(CreateBet())).WithMessage("Нельзя делать ставки не находясь в игре");
        }

        [Test]
        public void DoRightBets_Player_DoBet()
        {
            Player player = new Player();
            Game game = new Game();
            player.Enter(game);

            Assert.Throws<InvalidOperationException>(() => player.DoBet(CreateBet(diceValue: 7))).WithMessage("Значение ставки должно быть от 1 до 6");
        }

        [Test]
        public void NotEnoughChips_Player_DoBet()
        {
            Game game = Create.Game.Started();
            Player player = Create.Player.In(game).WithChips(3);

            Assert.Throws<InvalidOperationException>(() => player.DoBet(5.Chips().On(1))).WithMessage("Ты не можешь фишек больше поставить чем у тебя есть");
        }

        [Test]
        public void ChangeBetBegoreStartingGame_Player_ChangeBet()
        {
            Game game = Create.Game.Started();
            Player player = Create.Player.In(game).WithChips(100.Chips()).WithBet(CreateBet());
            Bet changedBet = 50.Chips().On(1);

            Assert.Throws<InvalidOperationException>(() => player.ChangeBet(changedBet)).WithMessage("Нельзя поменять ставку в игре которая уже началась");
        }

        private static Father Create;
    }

    public class Father
    {
        public GameFather Game = new GameFather();
        public PlayerFather Player = new PlayerFather();
    }


    public class PlayerFather
    {
        private Player player = new Player();

        public PlayerFather In(Game game)
        {
            player.Enter(game);
            return this;
        }

        public static implicit operator Player(PlayerFather father)
        {
            return father.player;
        }

        public PlayerFather WithBet(Bet bet)
        {
            player.DoBet(bet);
            return this;
        }

        public PlayerFather WithChips(int chipsCount)
        {
            player.BuyChips(chipsCount);
            return this;
        }
    }

    public class GameFather
    {
        private Game game = new Game();

        public GameFather Started()
        {
            game.isStarted = true;
            return this;
        }

        public static implicit operator Game(GameFather father)
        {
            return father.game;
        }
    }

    public class BaseTest
    {
        public Game CreateGame()
        {
            return new Game();
        }

        public Player CreatePlayer()
        {
            return new Player();
        }

        public Bet CreateBet(int diceValue = 1, int chipCount = 0)
        {
            return new Bet(diceValue, chipCount);
        }
    }

    [TestFixture]
    public class GameTests : BaseTest
    {
        [Test]
        public void EnterGame_Game_Max6Players()
        {
            var game = CreateGame();
            var player = CreatePlayer();
            var player2 = CreatePlayer();
            var player3 = CreatePlayer();
            var player4 = CreatePlayer();
            var player5 = CreatePlayer();
            var player6 = CreatePlayer();
            var player7 = CreatePlayer();

            player.Enter(game);
            player2.Enter(game);
            player3.Enter(game);
            player4.Enter(game);
            player5.Enter(game);
            player6.Enter(game);

            var e = Assert.Throws<InvalidOperationException>(() => player7.Enter(game));
            Assert.AreEqual("В игре число игроков максимальное, юный падован", e.Message);
        }
    }

    public static class ExteptionExtensions
    {
        public static void WithMessage(this Exception e, string expectedMessage)
        {
            Assert.AreEqual(expectedMessage, e.Message);
        }
    }

    public static class IntExtensions
    {
        public static int Chips(this int chips)
        {
            return chips;
        }

        public static Bet On(this int chips, int score)
        {
            return new Bet(diceValue: score, chipsCount: chips);
        }
    }
}