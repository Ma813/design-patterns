import React from 'react';
import Card from './Card';

export default function Deck({ deck, cardColors, onCardPlay }) {
    if (!deck || !deck.cards) return null;

    return (
        <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
            {deck.cards.map((card, idx) => (
                <Card
                    key={idx}
                    card={card}
                    index={idx} // Pass the index to Card component
                    onPlay={() => onCardPlay(idx)} // Pass both card and index
                    cardColors={cardColors}
                />
            ))}
        </div>
    );
}