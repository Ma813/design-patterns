import React, { useState, useEffect, useRef } from 'react';
import './App.css';
import * as signalR from '@microsoft/signalr';
import Deck from './components/Deck';
import Card from './components/Card'

function App() {
  const [connection, setConnection] = useState(null);
  const [messages, setMessages] = useState([]);
  const [buttonPresses, setButtonPresses] = useState([]);
  const [roomName, setRoomName] = useState('default-room');
  const [userName, setUserName] = useState('');
  const [isConnected, setIsConnected] = useState(false);
  const [deck, setDeck] = useState(null);
  const [players, setPlayers] = useState([]);
  const [topCard, setTopCard] = useState(null);
  const messagesEndRef = useRef(null);
  const [started, setStarted] = useState(false);
  const [hasJoined, setHasJoined] = useState(false);
  const [cardAmounts, setCardAmounts] = useState([]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, buttonPresses]);


  const connectToHub = async () => {
    try {
      console.log('Attempting to connect to SignalR hub...');

      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5000/chatHub', {
          skipNegotiation: true, // Important for CORS
          transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information) // Add logging
        .build();

      // Set up event handlers
      newConnection.on('ReceiveMessage', (user, message) => {
        console.log('Message received:', user, message);
        setMessages(prev => [...prev, { user, message, type: 'message', timestamp: new Date() }]);
      });

      newConnection.on("ReceiveDeck", (deck, topCard, players) => {
        setDeck(deck);
        console.log(deck)
        setTopCard(topCard);
        setPlayers(players);
        console.log(players)
      })

      newConnection.on('ReceiveButtonPress', (user, buttonName, timestamp) => {
        console.log('Button press received:', user, buttonName);
        setButtonPresses(prev => [...prev, { user, buttonName, timestamp: new Date(timestamp), type: 'button' }]);
      });

      newConnection.on('UserJoined', (players) => {
        setPlayers(players);
      });

      newConnection.on("GameStarted", (deck, topCard, cardAmounts) => {
        setStarted(true);
        setDeck(deck);
        setCardAmounts(cardAmounts);
        console.log(cardAmounts)
        setTopCard(topCard);

      });

      newConnection.on('UserLeft', (message) => {
        console.log('User left:', message);
        setMessages(prev => [...prev, { user: 'System', message, type: 'system', timestamp: new Date() }]);
      });

      newConnection.onclose((error) => {
        console.log('Connection closed:', error);
        setIsConnected(false);
      });

      newConnection.onreconnecting((error) => {
        console.log('Reconnecting:', error);
        setIsConnected(false);
      });

      newConnection.onreconnected((connectionId) => {
        console.log('Reconnected:', connectionId);
        setIsConnected(true);
      });

      // Start connection
      await newConnection.start();
      setConnection(newConnection);
      setIsConnected(true);

      console.log('Connected successfully! Connection ID:', newConnection.connectionId);

      return newConnection;
    } catch (error) {
      console.error('Connection failed:', error);
      alert(`Connection failed: ${error.message}`);
    }
  };

  const disconnectFromHub = async () => {
    if (connection) {
      try {
        await connection.stop();
        setConnection(null);
        setIsConnected(false);
        console.log('Disconnected successfully');
      } catch (error) {
        console.error('Disconnect failed:', error);
      }
    }
  };

  const joinRoom = async () => {
    let activeConnection = connection;

    if (!isConnected) {
      activeConnection = await connectToHub(); // get actual connection
    }

    if (activeConnection && roomName && userName) {
      try {
        await activeConnection.invoke('JoinRoom', roomName, userName);
        setMessages(prev => [...prev, {
          user: 'System',
          message: `You joined room: ${roomName}`,
          type: 'system',
          timestamp: new Date()
        }]);
        setHasJoined(true);
        console.log('Joined room:', roomName);
      } catch (error) {
        console.error('Join room failed:', error);
        alert(`Join room failed: ${error.message}`);
      }
    }
  };

  const startGame = async () => {
    if (connection) {
      try {
        await connection.invoke("StartGame", roomName, userName);
      } catch (error) {
        console.error("Start game failed:", error);
        alert(`Start game failed: ${error.message}`);
      }
    }
  };

  // Cleanup on component unmount
  useEffect(() => {
    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connection]);

  return (
    <div className="App">
      <header className="App-header">
        <h1>SignalR Chat Room</h1>

        <div className="connection-status" style={{
          backgroundColor: isConnected ? '#4CAF50' : '#f44336'
        }}>
          Status: {isConnected ? 'Connected' : 'Disconnected'}
          {connection && connection.connectionId && ` (ID: ${connection.connectionId.substring(0, 8)}...)`}
        </div>

        {!hasJoined && (
          <div className="controls">
            <input
              type="text"
              placeholder="Enter your name"
              value={userName}
              onChange={(e) => setUserName(e.target.value)}
              disabled={isConnected}
            />

            <input
              type="text"
              placeholder="Room name"
              value={roomName}
              onChange={(e) => setRoomName(e.target.value)}
            />
          </div>
        )}

        {!hasJoined && (
          <div className="room-controls">
            <button onClick={joinRoom} disabled={!roomName || !userName}>
              Join Room: {roomName}
            </button>
          </div>
        )}

        {hasJoined && !started && (
          <div className="start-game-controls">
            <button
              onClick={startGame}
              style={{
                backgroundColor: 'green',
                color: '#fff',
                fontWeight: 'bold',
                fontSize: '1.2rem',
                padding: '12px 24px',
                border: 'none',
                borderRadius: '8px',
                cursor: 'pointer',
                boxShadow: '0 4px 12px rgba(0,0,0,0.3)',
                transition: 'all 0.2s ease-in-out'
              }}
              onMouseOver={e => e.currentTarget.style.transform = 'scale(1.05)'}
              onMouseOut={e => e.currentTarget.style.transform = 'scale(1)'}
            >
              Start Game
            </button>
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
        {started && cardAmounts && Object.keys(cardAmounts).length > 0 && (
          <div className="player-amounts-container" style={{ margin: '20px 0' }}>
            <h3>Player Card Amounts:</h3>
            {Object.entries(cardAmounts).map(([playerName, amount]) => (
              <p key={playerName}>
                {playerName} {playerName === userName ? "(you)" : ""} has: {amount} cards
              </p>
            ))}
          </div>
        )}

        {topCard && (
          <div className="top-card-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Card card={topCard} />
          </div>
        )}

        {deck && (
          <div className="deck-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <h3>Deck:</h3>
            <Deck deck={deck} />
          </div>
        )}

        <div className="messages-container">
          <h3>Messages:</h3>
          <div className="messages">
            {messages.map((msg, index) => (
              <div key={index} className={`message ${msg.type}`}>
                <span className="timestamp">
                  {msg.timestamp.toLocaleTimeString()}
                </span>
                <strong>{msg.user}:</strong> {msg.message}
              </div>
            ))}
            <div ref={messagesEndRef} />
          </div>
        </div>

      </header>
    </div>
  );
}

export default App;