#pragma once

#include <string>
#include <vector>

#include "GameObject.h"
#include "ITickedObject.h"
#include "PlayPage.h"
#include "HouseBlock.h"

class PlayPage;

class DropHouseBlock : public GameObject, public ITickedObject
{
private:
	int step = 2;
	bool init = false;
protected:
	DropHouseBlock() {};
public:
	DropHouseBlock(std::shared_ptr<HouseBlock> owner, int step = 2);
	virtual void Init() override;
	virtual void Tick(int tick) override;
};
