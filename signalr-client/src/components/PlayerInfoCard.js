import React from 'react';

export default function PlayerCardInfo({ playerName, amount, isYou, connection, roomName }) {
    const buttonStyle = (color) => ({
        backgroundColor: color,
        color: '#fff',
        fontWeight: 'bold',
        padding: '8px 16px',
        border: 'none',
        borderRadius: '6px',
        cursor: 'pointer',
        margin: '5px',
        boxShadow: '0 2px 8px rgba(0,0,0,0.2)',
        transition: 'all 0.15s ease-in-out'
    });

    const handleInvoke = async (method, ...args) => {
        if (connection) {
            try {
                await connection.invoke(method, ...args);
            } catch (error) {
                console.error(`${method} failed:`, error);
                alert(`${method} failed: ${error.message}`);
            }
        }
    };

    return (
        <div
            className="player-card-info"
            style={{
                margin: '10px 0',
                padding: '10px 14px',
                borderRadius: '8px',
                backgroundColor: isYou ? '#e8f5e9' : '#f9f9f9',
                boxShadow: '0 2px 6px rgba(0,0,0,0.1)',
            }}
        >
            <p
                style={{
                    fontSize: '1rem',
                    fontWeight: isYou ? 'bold' : '500',
                    marginBottom: '8px',
                    color: isYou ? '#2e7d32' : '#333',
                }}
            >
                {playerName} {isYou && '(you)'} has: <span style={{ color: '#007bff' }}>{amount}</span> cards
            </p>

            <div>
                <button
                    style={buttonStyle('#007bff')}
                    onMouseOver={(e) => (e.currentTarget.style.transform = 'scale(1.05)')}
                    onMouseOut={(e) => (e.currentTarget.style.transform = 'scale(1)')}
                    onClick={() => handleInvoke('AnnoyPlayer', roomName, playerName, 'soundeffect')}
                >
                    🔊 Sound Effect
                </button>

                <button
                    style={buttonStyle('#6c757d')}
                    onMouseOver={(e) => (e.currentTarget.style.transform = 'scale(1.05)')}
                    onMouseOut={(e) => (e.currentTarget.style.transform = 'scale(1)')}
                    onClick={() => handleInvoke('ToggleMutePlayer', roomName, playerName)}
                >
                    🔇 Toggle Mute
                </button>

                <button
                    style={buttonStyle('#ffc107')}
                    onMouseOver={(e) => (e.currentTarget.style.transform = 'scale(1.05)')}
                    onMouseOut={(e) => (e.currentTarget.style.transform = 'scale(1)')}
                    onClick={() => handleInvoke('AnnoyPlayer', roomName, playerName, 'flashbang')}
                >
                    ⚡ Flashbang
                </button>
            </div>
        </div>
    );
}