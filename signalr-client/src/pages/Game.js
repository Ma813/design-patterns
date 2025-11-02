import { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import Deck from "../components/Deck";
import Card from "../components/Card";
import "./Game.css";
import PlayerCardInfo from "../components/PlayerInfoCard";
import Chat from "../components/Chat";

function App() {
  // Chat sidebar state
  const [isChatOpen, setIsChatOpen] = useState(true);
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
  const [botCount, setBotCount] = useState(0);
  const location = useLocation();

  function normalizeCard(card) {
    if (!card) return null;
    return {
      Color: card.Color || card.color || "gray",
      Name: card.Name || card.name || "?",
    };
  }

  function normalizeDeck(deck) {
    if (!deck) return null;
    return {
      username: deck.Username || deck.username,
      cards: (deck.Cards || deck.cards || []).map(normalizeCard),
    };
  }
    // Chat message state
    const [messages, setMessages] = useState([]);

  useEffect(() => {
    const urlParts = window.location.pathname.split("/");
    const room = urlParts[urlParts.length - 1];
    const name =
      location.state?.userName || `User${Math.floor(Math.random() * 10000)}`;
    const botCount = parseInt(location.state?.botCount) || 0;

    setUserName(name);
    setRoomName(room);
    setBotCount(botCount);
  }, []);

  useEffect(() => {
    if (roomName && userName) {
      const gameMode = parseInt(location.state?.gameMode) || 0;
      const cardGenerationMode = parseInt(location.state?.cardGenerationMode) || 0;
      const placementStrategy = parseInt(location.state?.placementStrategy) || 0;
      joinRoom(gameMode, cardGenerationMode, placementStrategy, botCount);
    }
  }, [roomName, userName, botCount]);

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
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      newConnection.on("GameStatus", (game) => {
        setDeck(normalizeDeck(game.playerDeck));
        setTopCard(normalizeCard(game.topCard));
        setPlayerAmounts(game.playersCardsCounts);
        setCurrentPlayer(game.currentPlayer);
        setError(null);
        console.log("Game status updated:", game);
      });

      newConnection.on("UserJoined", (players) => setPlayers(players));

      newConnection.on("GameStarted", (game) => {
        setStarted(true);
        setDeck(normalizeDeck(game.playerDeck));
        setTopCard(normalizeCard(game.topCard));
        setPlayerAmounts(game.playersCardsCounts);
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

     
      newConnection.on("Flashbang", () => {
                alert("Flashbang effect triggered!");
            });
      newConnection.on("PlaySound", (file) => {
                const audio = new Audio(`/sounds/${file}`);
                console.log("Playing sound:", file);
                audio.play();
            });
      newConnection.on("Error", (message) => {
        setError(message);
      });

      // Chat message handler
      newConnection.on("ReceiveMessage", (messageObj) => {
        // messageObj is { sender, text, timestamp }
        setMessages((prevMessages) => [...prevMessages, messageObj]);
      });

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

  const joinRoom = async (gameMode, cardGenerationMode, placementStrategy, botCount) => {
    let activeConnection = connection;

    if (!isConnected) {
      activeConnection = await connectToHub();
    }

    if (activeConnection && roomName && userName) {
      try {
        await activeConnection.invoke(
          "JoinRoom",
          roomName,
          userName,
          botCount,
          gameMode,
          cardGenerationMode,
          placementStrategy
        );
        setHasJoined(true);
        console.log("Joined room:", roomName);
      } catch (error) {
        console.error("Join room failed:", error);
      }
    }
  };

  const startGame = async () => {
    if (connection) {
      try {
        await connection.invoke("StartGame", roomName);
      } catch (error) {
        console.error("Start game failed:", error);
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
      }
    }
  }

  useEffect(() => {
    return () => {
      if (connection) connection.stop();
    };
  }, [connection]);

  return (
    <div className="Game">
        {/* Chat Toggle Button */}
      <button
      //TODO move to .css file
        style={{
          position: "fixed",
          top: 20,
          right: isChatOpen ? 340 : 20,
          zIndex: 2000,
          backgroundColor: "#222",
          color: "#fff",
          border: "none",
          borderRadius: "6px",
          padding: "10px 18px",
          boxShadow: "0 2px 8px rgba(0,0,0,0.2)",
          cursor: "pointer",
          fontWeight: 600,
          transition: "right 0.3s"
                }}
                onClick={() => setIsChatOpen(open => !open)}
            >
                {isChatOpen ? 'Close Chat' : 'Open Chat'}
            </button>
      <header className="Game-header">
        <h1>DOS Game Room</h1>

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

        {started && playerAmounts && Object.keys(playerAmounts).length > 0 && (
                    <div className="player-amounts-container" style={{ margin: '20px 0' }}>
                        <h3>Player Card Amounts:</h3>
                        {Object.entries(playerAmounts).map(([playerName, amount]) => (
                            <PlayerCardInfo
                                key={playerName}
                                playerName={playerName}
                                amount={amount}
                                isYou={playerName === userName}
                                connection={connection}
                                roomName={roomName}
                            />
                        ))}
                    </div>
                )}

                {players && !started && (
                    <div className="players-container" style={{ margin: '20px 0' }}>
                        <h3>Players in Room:</h3>

                        {players.map((player, idx) => (
                            <p key={idx}>
                                {player}
                            </p>
                        ))}
                    </div>
                )}

                <div className="annoy-player-container" style={{ margin: '20px 0' }}>
                    <button
                        style={{
                            backgroundColor: '#ff9800', // orange
                            color: '#fff',
                            fontWeight: 'bold',
                            padding: '8px 16px',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer',
                            margin: '5px',
                            boxShadow: '0 2px 8px rgba(0,0,0,0.2)',
                            transition: 'all 0.15s ease-in-out'
                        }}
                        onMouseOver={e => e.currentTarget.style.transform = 'scale(1.05)'}
                        onMouseOut={e => e.currentTarget.style.transform = 'scale(1)'}
                        onClick={async () => {
                            if (connection) {
                                try {
                                    await connection.invoke("AnnoyPlayers", roomName, "flashbang");
                                } catch (error) {
                                    console.error("Annoy players failed:", error);
                                    alert(`Annoy players failed: ${error.message}`);
                                }
                            }
                        }}
                    >
                        Annoy All Players with Flashbang
                    </button>
                </div>
                <div className="annoy-player-container" style={{ margin: '20px 0' }}>
                    <button
                        style={{
                            backgroundColor: '#28a745', // green
                            color: '#fff',
                            fontWeight: 'bold',
                            padding: '8px 16px',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer',
                            margin: '5px',
                            boxShadow: '0 2px 8px rgba(0,0,0,0.2)',
                            transition: 'all 0.15s ease-in-out'
                        }}
                        onMouseOver={e => e.currentTarget.style.transform = 'scale(1.05)'}
                        onMouseOut={e => e.currentTarget.style.transform = 'scale(1)'}
                        onClick={async () => {
                            if (connection) {
                                try {
                                    await connection.invoke("AnnoyPlayers", roomName, "soundeffect");
                                } catch (error) {
                                    console.error("Annoy players failed:", error);
                                    alert(`Annoy players failed: ${error.message}`);
                                }
                            }
                        }}
                    >
                        Annoy All Players with Sound Effect
                    </button>
                </div>

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
            {/* Chat Sidebar */}
            <div
                style={{
                    position: 'fixed',
                    top: 0,
                    right: 0,
                    width: 320,
                    height: '100vh',
                    background: '#fff',
                    boxShadow: '-3px 0 20px rgba(0,0,0,0.15)',
                    display: isChatOpen ? 'flex' : 'none',
                    flexDirection: 'column',
                    zIndex: 1500,
                    borderLeft: '1px solid #e0e0e0',
                    overflow: 'hidden',
                    transition: 'opacity 0.3s ease-in-out'
                }}
            >
                <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minHeight: 0 }}>
                    <Chat
                        connection={connection}
                        roomName={roomName}
                        userName={userName}
                        messages={messages}
                    />
                </div>
            </div>
    </div>
  );
}

export default App;
