import Card from "./Card";

export default function Deck({ deck, onCardPlay }) {
  if (!deck || !deck.cards) return null;

  return (
    <div
      style={{
        display: "flex",
        gap: "12px",
        alignItems: "center",
      }}
    >
      {deck.cards.map((card, idx) => (
        <Card key={idx} card={card} onPlay={() => onCardPlay(card)} />
      ))}
    </div>
  );
}
