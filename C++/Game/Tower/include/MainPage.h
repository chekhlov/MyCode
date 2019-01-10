#pragma once
#ifndef _MAINPAGE_H_
#define _MAINPAGE_H_
#include <memory>
#include "GamePage.h"
#include "MainScreen.h"

// Класс экрана приложения
class MainPage : public GamePage
{
private:
protected:
	std::shared_ptr<GameObject> background;
	std::shared_ptr<ButtonObject> btPlay;
	std::shared_ptr<ButtonObject> btHiscope;
	std::shared_ptr<ButtonObject> btHowPlay;
public:

	MainPage(std::shared_ptr<MainScreen> owner);
	virtual void Init() override;
	virtual ~MainPage();
	virtual void OnClickHowPlay();
	virtual void OnClickPlay();
	virtual void OnClickHighscope();
	//	virtual void Draw(SDL_Rect *dstRect = NULL) override;
};


#endif