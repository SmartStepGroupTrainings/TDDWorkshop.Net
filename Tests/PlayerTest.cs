﻿using System;
using Domain;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class PlayerTest : Test
    {
        [Test]
        public void SinglePlayer_EnterInGame_CanEnter()
        {
            var player = CreatePlayer();
            var game = CreateGame();

            player.EnterTo(game);

            Assert.IsTrue(player.HasEntered(game));
        }

        [Test]
        public void Player_ExitFromGame_CanExit()
        {
            var player = CreatePlayer();
            var game = CreateGame();
            player.EnterTo(game);

            player.ExitFromGame();

            Assert.IsFalse(player.HasEntered(game));
        }

        [Test]
        public void PlayerNotInGame_CanNotExitFromGame()
        {
           var player = CreatePlayer();

           var ex = Assert.Throws<Exception>(player.ExitFromGame);
           Assert.AreEqual("Выйти из игры не войдя, может только джедай", ex.Message);
        }

        [Test]
        public void PlayerInGame_EntersTheSameGame_ThrowsException()
        {
            var player = CreatePlayer();
            var game = CreateGame();
            player.EnterTo(game);

            var ex = Assert.Throws<Exception>(() => player.EnterTo(game));
            Assert.AreEqual("Войти в игру, будучи уже в игре, может только джедай",ex.Message);
        }

        [Test]
        public void TwoPlayers_CanEnterInGame()
        {
            var player1 = CreatePlayer();
            var player2 = CreatePlayer();

            var game = CreateGame();

            player1.EnterTo(game);
            player2.EnterTo(game);

            Assert.IsTrue(player2.HasEntered(game));
        }

        [Test]
        public void TwoPlayers_CanEnterInGame_FirstPlayerInGame()
        {
            var player1 = CreatePlayer();
            var player2 = CreatePlayer();

            var game = CreateGame();

            player1.EnterTo(game);
            player2.EnterTo(game);

            Assert.IsTrue(player1.HasEntered(game));
        }
    }
}
