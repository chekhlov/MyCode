#include "../stdafx.h"
#include "../include/TowerBlock.h"

TowerBlock::TowerBlock(std::shared_ptr<GameObject> owner, int ZOrder) : GameObject(owner, ZOrder)
{
	LoadResource("resource\\elGround.png");
}


TowerBlock::~TowerBlock()
{
	blocks.clear();
}

void TowerBlock::Init()
{
	auto page = std::static_pointer_cast<PlayPage>(owner);
	page->AddObject(std::static_pointer_cast<GameObject>(shared_from_this()));
	this->Position->x = (page->Size->cx - this->Size->cx) / 2;
	this->Position->y = (page->Size->cy - this->Size->cy) - 20;
	blocks.clear();
	move = 0;
	VisiblePosition = Position;
}

void TowerBlock::Move(int step)
{
	VisiblePosition->y += step;
	Position->y += step;
	for (auto block : blocks)
	{
		block->Position->y += step;
	}
}

void TowerBlock::Tick(int tick)
{
	if (move-- > 0) 
	{ 
		Move(2);
		move -= 2;
	}
}

void TowerBlock::Add(std::shared_ptr<DropHouseBlock> block)
{
	blocks.push_front(block);
	VisiblePosition->y -= block->Size->cy - 20;
	if (VisiblePosition->y < owner->Size->cy / 2) move = owner->Size->cy / 2 - VisiblePosition->y;
}

