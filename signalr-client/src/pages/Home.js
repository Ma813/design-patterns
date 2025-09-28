import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Home.css';

function Home() {
    const [userName, setUserName] = useState('');
    const [roomName, setRoomName] = useState('');
    const navigate = useNavigate();

    const joinRoom = () => {
        if (roomName) {
            navigate(`/game/${roomName}`, { state: { userName } });
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