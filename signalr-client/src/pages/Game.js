import { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import "./Game.css";
import * as signalR from "@microsoft/signalr";
import Deck from "../components/Deck";
import Card from "../components/Card";

function App() {
  const [connection, setConnection] = useState(null);
  const [roomName, setRoomName] = useState("");
  const [userName, setUserName] = useState("");
  const [isConnected, setIsConnected] = useState(false);
  const [deck, setDeck] = useState(null);
  const [players, setPlayers] = useState([]);
  const [topCard, setTopCard] = useState(null);
  const [started, setStarted] = useState(false);
  const [hasJoined, setHasJoined] = useState(false);
  const [playerAmounts, setPlayerAmounts] = useState([]);
  const [currentPlayer, setCurrentPlayer] = useState(null);
  const [error, setError] = useState(null);

  const location = useLocation();

  useEffect(() => {
    const urlParts = window.location.pathname.split("/");
    const room = urlParts[urlParts.length - 1];
    const name =
      location.state?.userName || `User${Math.floor(Math.random() * 10000)}`;
    setUserName(name);
    setRoomName(room);
  }, []);

  useEffect(() => {
    if (roomName && userName) {
      const gameMode = location.state?.gameMode || "Classic";
      const cardGenerationMode = location.state?.cardGenerationMode || 0;
      const placementStrategy = location.state?.placementStrategy || 0;
      joinRoom(gameMode, cardGenerationMode, placementStrategy);
    }
  }, [roomName, userName]);

  const connectToHub = async () => {
    try {
      console.log(
        "Attempting to connect to SignalR hub... Room:",
        roomName,
        "User:",
        userName
      );

      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/gameHub", {
          skipNegotiation: true, // Important for CORS
          transport: signalR.HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information) // Add logging
        .build();

      // Set up event handlers

      newConnection.on("GameStatus", (game) => {
        setDeck(game.playerDeck);
        setTopCard(game.topCard);
        setPlayerAmounts(game.playerAmounts);
        setCurrentPlayer(game.currentPlayer);
        setError(null); // Clear previous errors
        console.log("Game status updated:", game);
      });

      newConnection.on("UserJoined", (players) => {
        setPlayers(players);
      });

      newConnection.on("GameStarted", (game) => {
        setStarted(true);
        setDeck(game.playerDeck);
        setPlayerAmounts(game.playerAmounts);
        setTopCard(game.topCard);
        setCurrentPlayer(game.currentPlayer);
      });

      newConnection.on("GameEnded", (message) => {
        console.log("Game ended:", message);
        alert(`Game ended: ${message}`);
        setStarted(false);
        setDeck(null);
        setPlayerAmounts([]);
        setTopCard(null);
        setCurrentPlayer(null);
      });

      newConnection.on("UserLeft", (message) => {
        console.log("User left:", message);
      });

      newConnection.onclose((error) => {
        console.log("Connection closed:", error);
        setIsConnected(false);
      });

      newConnection.onreconnecting((error) => {
        console.log("Reconnecting:", error);
        setIsConnected(false);
      });

      newConnection.onreconnected((connectionId) => {
        console.log("Reconnected:", connectionId);
        setIsConnected(true);
      });

      newConnection.on("Error", (message) => {
        setError(message);
      });

      // Start connection
      await newConnection.start();
      setConnection(newConnection);
      setIsConnected(true);

      console.log(
        "Connected successfully! Connection ID:",
        newConnection.connectionId
      );

      return newConnection;
    } catch (error) {
      console.error("Connection failed:", error);
      alert(`Connection failed: ${error.message}`);
    }
  };

  const disconnectFromHub = async () => {
    if (connection) {
      try {
        await connection.stop();
        setConnection(null);
        setIsConnected(false);
        console.log("Disconnected successfully");
      } catch (error) {
        console.error("Disconnect failed:", error);
      }
    }
  };

  const joinRoom = async (gameMode, cardGenerationMode, placementStrategy) => {
    let activeConnection = connection;

    if (!isConnected) {
      activeConnection = await connectToHub(); // get actual connection
    }

    if (activeConnection && roomName && userName) {
      try {
        await activeConnection.invoke(
          "JoinRoom",
          roomName,
          userName,
          gameMode,
          cardGenerationMode,
          placementStrategy
        );
        setHasJoined(true);
        console.log("Joined room:", roomName);
      } catch (error) {
        console.error("Join room failed:", error);
        alert(`Join room failed: ${error.message}`);
      }
    }
  };

  const startGame = async () => {
    if (connection) {
      try {
        await connection.invoke("StartGame", roomName);
      } catch (error) {
        console.error("Start game failed:", error);
        alert(`Start game failed: ${error.message}`);
      }
    }
  };

  async function handleCardPlay(card) {
    console.log("Card clicked:", card);
    if (connection && started && currentPlayer === userName) {
      try {
        await connection.invoke("PlayCard", roomName, userName, card);
      } catch (error) {
        console.error("Play card failed:", error);
        alert(`Play card failed: ${error.message}`);
      }
    }
  }

  // Cleanup on component unmount
  useEffect(() => {
    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connection]);

  return (
    <div className="Game">
      <header className="Game-header">
        <h1>Dos game room</h1>

        {hasJoined && !started && (
          <div className="start-game-controls">
            <button
              onClick={startGame}
              style={{
                backgroundColor: "green",
                color: "#fff",
                fontWeight: "bold",
                fontSize: "1.2rem",
                padding: "12px 24px",
                border: "none",
                borderRadius: "8px",
                cursor: "pointer",
                boxShadow: "0 4px 12px rgba(0,0,0,0.3)",
                transition: "all 0.2s ease-in-out",
              }}
              onMouseOver={(e) =>
                (e.currentTarget.style.transform = "scale(1.05)")
              }
              onMouseOut={(e) => (e.currentTarget.style.transform = "scale(1)")}
            >
              Start Game
            </button>
          </div>
        )}

        {error && (
          <div
            className="error-message"
            style={{ color: "red", margin: "10px 0" }}
          >
            <strong>Error:</strong> {error}
          </div>
        )}

        {players && !started && (
          <div className="players-container" style={{ margin: "20px 0" }}>
            <h3>Players in Room:</h3>

            {players.map((player, idx) => (
              <p key={idx}>{player}</p>
            ))}
          </div>
        )}
        {started && playerAmounts && Object.keys(playerAmounts).length > 0 && (
          <div
            className="player-amounts-container"
            style={{ margin: "20px 0" }}
          >
            <h3>Player Card Amounts:</h3>
            {Object.entries(playerAmounts).map(([playerName, amount]) => (
              <p key={playerName}>
                {playerName} {playerName === userName ? "(you)" : ""} has:{" "}
                {amount} cards
              </p>
            ))}
          </div>
        )}

        {topCard && (
          <div
            className="top-card-container"
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
            }}
          >
            <Card card={topCard} />
          </div>
        )}

        {currentPlayer && (
          <div
            className="current-player-container"
            style={{ margin: "20px 0" }}
          >
            <h3>
              Current Player: {currentPlayer}{" "}
              {currentPlayer === userName ? "(you)" : ""}
            </h3>
          </div>
        )}

        {started && currentPlayer === userName && (
          <div className="draw-card-controls" style={{ margin: "20px 0" }}>
            <button
              onClick={async () => {
                if (connection) {
                  try {
                    await connection.invoke("DrawCard", roomName, userName);
                  } catch (error) {
                    console.error("Draw card failed:", error);
                    alert(`Draw card failed: ${error.message}`);
                  }
                }
              }}
              style={{
                backgroundColor: "#1890ff",
                color: "#fff",
                fontWeight: "bold",
                fontSize: "1.1rem",
                padding: "10px 22px",
                border: "none",
                borderRadius: "8px",
                cursor: "pointer",
                boxShadow: "0 2px 8px rgba(0,0,0,0.2)",
                transition: "all 0.2s ease-in-out",
              }}
              onMouseOver={(e) =>
                (e.currentTarget.style.transform = "scale(1.05)")
              }
              onMouseOut={(e) => (e.currentTarget.style.transform = "scale(1)")}
            >
              Draw Card
            </button>
          </div>
        )}

        {deck && (
          <div
            className="deck-container"
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
            }}
          >
            <h3>Deck:</h3>
            <Deck deck={deck} onCardPlay={handleCardPlay} />
          </div>
        )}
      </header>
    </div>
  );
}

export default App;
