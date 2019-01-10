#pragma once

#ifndef _GAME_H_
#define _GAME_H_

#include "GraphicEngine.h"
#include "GameScreen.h"
#include "MainPage.h"

class Game
{
public:
	static const int GE_TICK = 0x3000;
	static const int GE_DRAW = 0x3002;
	static const int GE_VK_ESC = 0x301B;
	static const int GE_VK_SPACE = 0x3020;
	static const int GE_VK_ENTER = 0x300D;
	int tick = 0;
	int fps = 0;
protected:
	std::shared_ptr<IGraphicEngine> engine = nullptr;
	bool quit = false;
	SIZE windowSize;
	std::shared_ptr<GameScreen> mainScreen = nullptr;
	void ProcessEvent(SDL_Event &event);

	std::shared_ptr<TextObject> frameText = nullptr;
	float CalcFps();
public:
	Game(int cx = GraphicEngine::GE_UseMaxSize, int cy = GraphicEngine::GE_UseMaxSize);
	void InitEngine(int cx = 0, int cy = 0);
	std::shared_ptr<IGraphicEngine> GetEngine();

	virtual ~Game();
	virtual std::shared_ptr<GameScreen> PrepareMainScreen(std::shared_ptr<IGraphicEngine> engine);
	int Execute();
	void Sync();

	void Exit();
};

#endif