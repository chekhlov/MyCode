#pragma once
#ifndef _MAINSCREEN_H_
#define _MAINSCREEN_H_
#include <memory>
#include <list>
#include "GraphicEngine.h"
#include "GameScreen.h"

// Класс экрана приложения
class MainScreen : public GameScreen 
{
	enum screenState { ssNormal, ssSwitchPage };
private:
protected:
	std::shared_ptr<GamePage> mainPage = nullptr;
	std::shared_ptr<GamePage> hiscopePage = nullptr;
	std::shared_ptr<GamePage> playPage = nullptr;
	std::shared_ptr<GamePage> howPlayPage = nullptr;
	std::shared_ptr<GamePage> prevPage = nullptr;
	virtual void ProcessEvent(int x, int y, MouseState state) override;
	int mainScreenState = 0;
public:
	MainScreen(std::shared_ptr<IGraphicEngine> owner);
	virtual void Init() override;
	void MainPage();
	void PlayPage();
	void HowPlayPage();
	void HiscopePage();
	virtual void Tick(int tick) override;
	virtual void SetPage(std::shared_ptr<GamePage> page);

	virtual ~MainScreen();
	friend class MainPage;
};


#endif