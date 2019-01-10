#pragma once
#ifndef _GAMESCREEN_H_
#define _GAMESCREEN_H_
#include <memory>
#include <list>
#include "GraphicResource.h"
#include "GraphicEngine.h"
#include "GamePage.h"
#include "ClickableObject.h"
#include "ITickedObject.h"

class GamePage;

// Класс экрана приложения
class GameScreen : public GameObject, public ClickableObject, public ITickedObject
{
private:
	std::list<std::shared_ptr<GamePage>> pages;	// Все страницы игры
protected:
	std::shared_ptr<GamePage> ActivePage;// текущая активная страница
	void SetActivePage(std::shared_ptr<GamePage> page);
public:
	GameScreen(std::shared_ptr<IGraphicEngine> owner);
	virtual ~GameScreen();

	virtual void Init() override {};
	virtual void Tick(int tick) override  {};
	virtual std::shared_ptr<GamePage> AddPage();
	virtual void AddPage(std::shared_ptr<GamePage> page);

//	virtual void Draw() override;
};


#endif