import React, { useState, useEffect } from 'react';

const Card = ({ card, index, onPlay, cardColors }) => { // Add index prop
    const colorMap = cardColors || {
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
        const baseColor = colorMap[card.color.toLowerCase()] || '#bfbfbf';
        setBg(lightenColor(baseColor, 20)); // brighten by 20
    };
    
    const handleMouseLeave = () => {
        setBg(colorMap[card.color] || '#d9d9d9');
    };

    const handleClick = () => {
        if (onPlay) {
            onPlay(index); // Pass both card and index
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
                cursor: 'pointer',
                fontSize: card.name?.length > 2 ? 14 : 24
            }}
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
            onClick={handleClick}
        >
            {card.name?.replaceAll('+', '\n')}
        </div>
    );
};

export default Card;

function lightenColor(color, percent) {
    // Convert hex to integer
    const num = parseInt(color.replace('#',''), 16);

    // Extract RGB channels
    let r = (num >> 16) & 0xFF;
    let g = (num >> 8) & 0xFF;
    let b = num & 0xFF;

    // Increase each channel by percent
    r = Math.min(255, Math.max(0, r + percent));
    g = Math.min(255, Math.max(0, g + percent));
    b = Math.min(255, Math.max(0, b + percent));

    // Convert back to hex
    return `#${r.toString(16).padStart(2,'0')}${g.toString(16).padStart(2,'0')}${b.toString(16).padStart(2,'0')}`;
}