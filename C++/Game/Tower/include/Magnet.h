#pragma once

#include <string>
#include <vector>

#include "GameObject.h"
#include "ITickedObject.h"
#include "PlayPage.h"
#include "HouseBlock.h"
#include "DropHouseBlock.h"

class PlayPage;
class DropHouseBlock;

class Magnet : public GameObject, public ITickedObject
{
	enum MangetState { msShowing, msMoving, msUnshowing, msUnvisible };
	int step = 2;
	int zorder = 0;
	int delay = 0;
private:
protected:
	std::shared_ptr<HouseBlock> houseBlock = nullptr;
	MangetState magnetState;
	void MoveHouseBlock();
	void NewBlock();
public:
	Magnet(std::shared_ptr<PlayPage> owner, int ZOrder = 0);
	virtual void Init() override;
	virtual void Tick(int tick) override;
	std::shared_ptr<DropHouseBlock> DropHouse();
};
