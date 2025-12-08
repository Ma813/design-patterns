import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import Game from './pages/Game';
import Console from './pages/Console';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/game/:roomName" element={<Game />} />
        <Route path="console" element={<Console />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;