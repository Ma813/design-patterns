import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Home.css';

function Home() {
    const [userName, setUserName] = useState('');
    const [roomName, setRoomName] = useState('');
    const [gameMode, setGameMode] = useState("Classic");
    const [placementStrategy, setPlacementStrategy] = useState("UnoPlacementStrategy");

    const navigate = useNavigate();

    const joinRoom = () => {
        console.log("joining with",placementStrategy)
        if (roomName) {
            navigate(`/game/${roomName}`, { 
                state: { 
                    userName, 
                    gameMode,
                    placementStrategy 
                } 
            });
        }
    };

    return (
        <div className="home-container">
            <div className="home-card">
                <h1 className="home-title">Join a Room</h1>
                
                <div className="form-group">
                    <label htmlFor="userName" className="form-label">Your Name</label>
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
                    <label htmlFor="roomName" className="form-label">Room Name</label>
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
                    <label htmlFor="gameMode" className="form-label">Game Mode</label>
                    <select
                        id="gameMode"
                        className="form-input"
                        value={gameMode}
                        onChange={(e) => setGameMode(e.target.value)}
                    >
                        <option value="Classic">Classic</option>
                        <option value="Endless">Endless</option>
                        <option value="DrawToMatch">Draw to Match</option>
                    </select>
                </div>

                <div className="form-group">
                    <label htmlFor="placementStrategy" className="form-label">Card Placement Strategy</label>
                    <select
                        id="placementStrategy"
                        className="form-input"
                        value={placementStrategy}
                        onChange={(e) => setPlacementStrategy(e.target.value)}
                    >
                        <option value="UnoPlacementStrategy">Uno Standard</option>
                        <option value="AdjacentNumberPlacementStrategy">Adjacent Number</option>
                        <option value="ColorOnlyPlacementStrategy">Color Only</option>
                        <option value="NumberOnlyPlacementStrategy">Number Only</option>
                    </select>
                </div>

                <div>
                    <label className="note-label" style={{ marginTop: '8px', color: '#888', fontSize: '0.9em', display: 'block' }}>
                        NOTE: only the first player to join the room can set the game mode and placement strategy. Subsequent players will join with the already selected options.
                    </label>
                </div>

                <button
                    onClick={joinRoom}
                    disabled={!roomName}
                    className="join-button"
                >
                    Join Room
                </button>
            </div>
        </div>
    );
}

export default Home;
