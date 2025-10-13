import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Home.css";

function Home() {
  const [userName, setUserName] = useState("");
  const [roomName, setRoomName] = useState("");
  const [gameMode, setGameMode] = useState(0);
  const [cardGenerationMode, setCardGenerationMode] = useState(0);
  const [placementStrategy, setPlacementStrategy] = useState(0);

  const navigate = useNavigate();

  const joinRoom = () => {
    if (roomName) {
      navigate(`/game/${roomName}`, {
        state: {
          userName,
          gameMode,
          cardGenerationMode,
          placementStrategy,
        },
      });
    }
  };

  return (
    <div className="home-container">
      <div className="home-card">
        <h1 className="home-title">Join a Room</h1>
        <div className="form-group">
          <label htmlFor="userName" className="form-label">
            Your Name
          </label>
          <input
            id="userName"
            type="text"
            placeholder="Enter your name"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label htmlFor="roomName" className="form-label">
            Room Name
          </label>
          <input
            id="roomName"
            type="text"
            placeholder="Enter room name"
            value={roomName}
            onChange={(e) => setRoomName(e.target.value)}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label htmlFor="gameMode" className="form-label">
            Game Mode
          </label>
          <select
            id="gameMode"
            className="form-input"
            value={gameMode}
            onChange={(e) => setGameMode(e.target.value)}
          >
            <option value="0">Classic</option>
            <option value="1">Endless</option>
            <option value="2">Draw to Match</option>
          </select>
        </div>
        <div className="form-group">
          <label htmlFor="cardGenerationMode" className="form-label">
            Card Generation mode
          </label>
          <select
            id="cardGenerationMode"
            className="form-input"
            value={cardGenerationMode}
            onChange={(e) => setCardGenerationMode(e.target.value)}
          >
            <option value="0">Standard</option>
            <option value="1">All cards are the same color</option>
            <option value="2">Generate only number cards</option>
            <option value="3">Generate only action cards</option>
          </select>
        </div>
        <div className="form-group">
          <label htmlFor="placementStrategy" className="form-label">
            Card Placement Strategy
          </label>
          <select
            id="placementStrategy"
            className="form-input"
            value={placementStrategy}
            onChange={(e) => setPlacementStrategy(e.target.value)}
          >
            <option value="0">Uno Standard</option>
            <option value="1">Color only</option>
            <option value="2">Face only</option>
            <option value="3">Adjacent Number</option>
          </select>
        </div>
        <div>
          <label
            className="note-label"
            style={{
              marginTop: "8px",
              color: "#888",
              fontSize: "0.9em",
              display: "block",
            }}
          >
            NOTE: only the first player to join the room can set the game mode.
            Subsequent players will join with the already selected mode.
          </label>
        </div>
        <button onClick={joinRoom} disabled={!roomName} className="join-button">
          Join Room
        </button>
      </div>
    </div>
  );
}

export default Home;
