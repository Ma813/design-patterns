import React from 'react';
import Card from './Card';

export default function Deck({ deck, cardColors, onCardPlay }) {
    if (!deck) return null;

    return (
        <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
            {deck.map((card, idx) => (
                // <><p>{JSON.stringify(card)}</p>
                <Card
                    key={idx}
                    card={card}
                    index={idx} // Pass the index to Card component
                    onPlay={() => onCardPlay(idx)} // Pass both card and index
                    cardColors={cardColors} />
            ))}
        </div>
    );
}