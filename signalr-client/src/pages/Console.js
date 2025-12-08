import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import "./Console.css";

export default function Console({ roomName, userName }) {
    const [input, setInput] = useState("");
    const [history, setHistory] = useState([]);
    const [connection, setConnection] = useState(null);
    const outputRef = useRef(null);
    const endRef = useRef(null);

    const API_URL = process.env.REACT_APP_API || "http://localhost:5000";

    useEffect(() => {
        const consoleConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/GameHub`, {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        consoleConnection
            .start()
            .then(() => {
                setConnection(consoleConnection);
                setHistory(prev => [...prev, "SYSTEM: Connected to Hub"]);
            })
            .catch(err => {
                console.error("Connection failed: ", err);
                setHistory(prev => [...prev, `SYSTEM: Connection failed: ${err.message || err}`]);
            });

        // Event handlers
        consoleConnection.on("ReceiveCommandOutput", (msg) => {
            setHistory(prev => [...prev, msg]);
        });

        consoleConnection.on("SystemMessage", (msg) => {
            setHistory(prev => [...prev, `SYSTEM: ${msg}`]);
        });

        consoleConnection.onclose((err) => {
            setHistory(prev => [...prev, `SYSTEM: Connection closed. ${err ? err.message : ""}`]);
        });

        return () => {
            try { consoleConnection.stop(); } catch { }
        };
    }, [API_URL]);

    // Robust autoscroll: scrollIntoView on the anchor element, after paint
    useEffect(() => {
        if (!endRef.current) return;

        // Wait until layout/paint complete
        const id = requestAnimationFrame(() => {
            try {
                endRef.current.scrollIntoView({ behavior: "auto", block: "end", inline: "nearest" });
                // Also ensure parent has scrolled to bottom
                if (outputRef.current) {
                    outputRef.current.scrollTop = outputRef.current.scrollHeight;
                }
            } catch (e) {
                // fallback
                if (outputRef.current) {
                    outputRef.current.scrollTop = outputRef.current.scrollHeight;
                }
            }
        });

        return () => cancelAnimationFrame(id);
    }, [history]);

    async function sendCommand() {
        if (!input.trim() || !connection) return;

        setHistory(prev => [...prev, `> ${input}`]);

        try {
            await connection.invoke("Command", input);
        } catch (err) {
            setHistory(prev => [...prev, `ERROR: ${err.message || err}`]);
        }

        setInput("");
    }

    return (
        <div className="terminal-container">
            <div className="terminal-output" ref={outputRef}>
                {history.map((line, i) => (
                    <div key={i} className="terminal-line">{line}</div>
                ))}
                {/* Anchor element to scroll into view */}
                <div ref={endRef} />
            </div>

            <div className="terminal-input-row">
                <span className="prompt">&gt;</span>
                <input
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onKeyDown={(e) => e.key === "Enter" && sendCommand()}
                    className="terminal-input"
                    placeholder="Type command…"
                />
            </div>
        </div>
    );
}