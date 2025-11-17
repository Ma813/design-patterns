import React, { useState, useEffect, useRef } from "react";

const Chat = ({ connection, roomName, userName }) => {
    const [messages, setMessages] = useState([]);
    const [text, setText] = useState("");
    const messagesEndRef = useRef(null);

    useEffect(() => {
        if (!connection) return;

        const handleReceiveMessage = (msg) => {
            setMessages((prevMessages) => [...prevMessages, msg]);
        };

        connection.on("ReceiveMessage", handleReceiveMessage);

        return () => {
            connection.off("ReceiveMessage", handleReceiveMessage);
        };
    }, [connection]);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages]);

    const sendMessage = async () => {
        if (text.trim() === "") return;
        try {
            await connection.invoke("SendMessage", roomName, userName, text);
            setText("");
        } catch (e) {
            console.error("Send message failed: ", e);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === "Enter") {
            e.preventDefault();
            sendMessage();
        }
    };

    return (
        <div
            style={{
                maxWidth: "600px",
                margin: "20px auto",
                padding: "20px",
                backgroundColor: "white",
                borderRadius: "12px",
                boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
                display: "flex",
                flexDirection: "column",
                height: "80vh",
            }}
        >
            <div
                style={{
                    flexGrow: 1,
                    overflowY: "auto",
                    padding: "15px",
                    borderRadius: "8px",
                    backgroundColor: "#f5f7fa",
                    border: "1px solid #e0e0e0",
                    marginBottom: "12px",
                    display: "flex",
                    flexDirection: "column",
                }}
            >
                {messages.map((msg, idx) => {
                    const isUser = msg.sender === userName;
                    return (
                        <div
                            key={idx}
                            style={{
                                alignSelf: isUser ? "flex-end" : "flex-start",
                                backgroundColor: isUser ? "#d0e7ff" : "#e8e8e8",
                                color: "#333",
                                padding: "10px 14px",
                                borderRadius: "16px",
                                marginBottom: "10px",
                                maxWidth: "75%",
                                boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
                            }}
                        >
                            <div style={{ fontWeight: "600", marginBottom: "4px" }}>
                                {msg.sender}
                            </div>
                            <div>{msg.text}</div>
                            <div
                                style={{
                                    fontSize: "11px",
                                    color: "#777",
                                    marginTop: "6px",
                                    textAlign: "right",
                                }}
                            >
                                {msg.timestamp ? new Date(msg.timestamp).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) : ""}
                            </div>
                        </div>
                    );
                })}
                <div ref={messagesEndRef} />
            </div>
            <div
                style={{
                    display: "flex",
                    gap: "10px",
                    alignItems: "center",
                }}
            >
                <textarea
                    rows={2}
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    onKeyPress={handleKeyPress}
                    placeholder="Type a message..."
                    style={{
                        resize: "none",
                        padding: "12px",
                        borderRadius: "20px",
                        border: "1px solid #ccc",
                        fontSize: "14px",
                        fontFamily: "inherit",
                        flexGrow: 1,
                        boxShadow: "inset 0 1px 3px rgba(0,0,0,0.1)",
                        outline: "none",
                    }}
                />
                <button
                    onClick={sendMessage}
                    style={{
                        backgroundColor: "#1890ff",
                        color: "white",
                        padding: "12px 24px",
                        borderRadius: "20px",
                        cursor: "pointer",
                        border: "none",
                        fontSize: "14px",
                        fontFamily: "inherit",
                        boxShadow: "0 2px 6px rgba(24,144,255,0.6)",
                        transition: "background-color 0.3s ease",
                    }}
                    onMouseOver={e => (e.currentTarget.style.backgroundColor = "#147acc")}
                    onMouseOut={e => (e.currentTarget.style.backgroundColor = "#1890ff")}
                >
                    Send
                </button>
            </div>
        </div>
    );
};

export default Chat;
