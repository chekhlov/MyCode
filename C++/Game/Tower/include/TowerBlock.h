#pragma once

#include <string>
#include <vector>

#include "GameObject.h"
#include "ITickedObject.h"
#include "DropHouseBlock.h"

class TowerBlock : public GameObject, public ITickedObject
{
private:
	std::list<std::shared_ptr<DropHouseBlock>> blocks;
	int move = 0;
protected:
	void Move(int step);
public:
	Property<POINT> VisiblePosition;

	TowerBlock(std::shared_ptr<GameObject> owner, int ZOrder = 0);
	virtual ~TowerBlock();
	virtual void Init() override;
	virtual void Tick(int tick) override;
	void Add(std::shared_ptr<DropHouseBlock> block);
};
