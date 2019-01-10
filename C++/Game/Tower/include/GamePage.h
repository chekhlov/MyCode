#pragma once
#ifndef _GAMEPAGE_H_
#define _GAMEPAGE_H_

#include "GraphicResource.h"
#include "GameObject.h"
#include "TextObject.h"
#include "ClickableObject.h"
#include "ButtonObject.h"
#include "ITickedObject.h"

#include "GameScreen.h"

// Класс страницы игры
class GameScreen;

class GamePage : public GameObject, public ClickableObject, public ITickedObject
{
private:
protected:
	GamePage() {};
public:
	GamePage(std::shared_ptr<GameScreen> owner);
	virtual ~GamePage();
	virtual void Init() override {};
	virtual void Tick(int tick) override {};
};

#endif