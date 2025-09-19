import React, { useState } from 'react';

const Card = ({ card }) => {
    const colorMap = {
        red: '#ff4d4f',
        green: '#52c41a',
        yellow: '#faad14',
        blue: '#1890ff'
    };
    const [bg, setBg] = useState(colorMap[card.color] || '#d9d9d9');
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
            }}
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
        >
            {card.digit}
        </div>
    );
};

export default Card;
