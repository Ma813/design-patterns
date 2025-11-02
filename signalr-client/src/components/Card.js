import { useState, useEffect } from "react";

const Card = ({ card, onPlay }) => {
  const color = card.Color || card.color || "gray";
  const name = card.Name || card.name || "?";

  const colorMap = {
    red: "#ff4d4f",
    green: "#52c41a",
    yellow: "#faad14",
    blue: "#1890ff",
  };

  const [bg, setBg] = useState(colorMap[color.toLowerCase()] || "#d9d9d9");

  useEffect(() => {
    setBg(colorMap[color.toLowerCase()] || "#d9d9d9");
  }, [card.Color]);

  const handleMouseEnter = () => {
    switch (card.Color) {
      case "Red":
        setBg("#ff7875");
        break;
      case "Green":
        setBg("#73d13d");
        break;
      case "Yellow":
        setBg("#ffd666");
        break;
      case "Blue":
        setBg("#40a9ff");
        break;
      default:
        setBg("#bfbfbf");
    }
  };
  const handleMouseLeave = () =>
    setBg(colorMap[color.toLowerCase()] || "#d9d9d9");

  const handleClick = () => {
    if (onPlay) {
      onPlay(card);
    }
  };

  return (
    <div
      style={{
        background: bg,
        color: "#fff",
        width: 48,
        height: 72,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        borderRadius: 8,
        fontSize: 24,
        fontWeight: "bold",
        boxShadow: "0 2px 8px rgba(0,0,0,0.15)",
        transition: "background 0.2s",
        userSelect: "none",
        cursor: "pointer",
      }}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      onClick={handleClick}
    >
      {name}
    </div>
  );
};

export default Card;
