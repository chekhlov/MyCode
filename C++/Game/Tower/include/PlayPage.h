#pragma once
#ifndef _PLAYPAGE_H_
#define _PLAYPAGE_H_
#include <memory>
#include "GamePage.h"
#include "MainScreen.h"
#include "Magnet.h"
#include "DropHouseBlock.h"
#include "TowerBlock.h"

class Magnet;
class TowerBlock;
class DropHouseBlock;

// Класс экрана приложения
class PlayPage : public GamePage
{
private:
protected:
	std::shared_ptr<GameObject> background;
	std::shared_ptr<GameObject> sun;
	std::shared_ptr<GameObject> smallBaloon;
	std::shared_ptr<GameObject> clouds1;
	std::shared_ptr<GameObject> clouds2;

	std::shared_ptr<Magnet> magnet;
	std::shared_ptr<TowerBlock> towers;
	std::shared_ptr<DropHouseBlock> droppedHouseBlock;
//	std::shared_ptr<GameObject> bigBaloon;

	virtual void OnClickClose();
	virtual void OnClickToDrop();
	void DroppedBlockAnalyze();
public:

	PlayPage(std::shared_ptr<MainScreen> owner);
	virtual void Init() override;
	virtual void Tick(int tick) override;
	virtual ~PlayPage();
};


#endif