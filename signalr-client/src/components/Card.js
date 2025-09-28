import React, { useState, useEffect } from 'react';

const Card = ({ card, onPlay }) => {
    const colorMap = {
        red: '#ff4d4f',
        green: '#52c41a',
        yellow: '#faad14',
        blue: '#1890ff'
    };
    const [bg, setBg] = useState(colorMap[card.color] || '#d9d9d9');
     // Add useEffect to update bg when card.color changes
    useEffect(() => {
        setBg(colorMap[card.color] || '#d9d9d9');
    }, [card.color]);

    
    const handleMouseEnter = () => {
        switch (card.color) {
            case 'red':
                setBg('#ff7875');
                break;
            case 'green':
                setBg('#73d13d');
                break;
            case 'yellow':
                setBg('#ffd666');
                break;
            case 'blue':
                setBg('#40a9ff');
                break;
            default:
                setBg('#bfbfbf');
        }
    };
    
    const handleMouseLeave = () => {
        setBg(colorMap[card.color] || '#d9d9d9');
    };

    const handleClick = () => {
        if (onPlay) {
            onPlay();
        }
    };

    return (
        <div
            style={{
                background: bg,
                color: '#fff',
                width: 48,
                height: 72,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                borderRadius: 8,
                fontSize: 24,
                fontWeight: 'bold',
                boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
                transition: 'background 0.2s',
                userSelect: 'none',
                cursor: 'pointer', // Add cursor pointer to indicate it's clickable
            }}
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
            onClick={handleClick} // Add onClick handler here
        >
            {card.digit}
        </div>
    );
};

export default Card;