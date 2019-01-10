#include "../stdafx.h"
#include "../include/Magnet.h"
#include "../include/HouseBlock.h"
#include <math.h>

Magnet::Magnet(std::shared_ptr<PlayPage> owner, int ZOrder) : GameObject(owner, ZOrder)
{
//	Count(this);
	LoadResource("resource\\magnet.png");
};

void Magnet::NewBlock()
{
	houseBlock = std::shared_ptr<HouseBlock>(new HouseBlock(owner, zorder--));
	int rand = std::rand() % 10;
	houseBlock->LoadResource(fmt::format("resource\\elHouse{0}.png", rand));
	owner->AddObject(houseBlock);
}


void Magnet::Init()
{
	auto page = std::static_pointer_cast<PlayPage>(owner);
	page->AddObject(std::static_pointer_cast<GameObject>(shared_from_this()));
	zorder = ZOrder + 100;

	NewBlock();

	magnetState = msShowing;

	this->Position->x = std::rand() % (page->Size->cx - this->Size->cx - 60);
	this->Position->y = - this->Size->cy - houseBlock->Size->cy;
	houseBlock->Position->x = this->Position->x + (this->Size->cx - houseBlock->Size->cx) / 2;
	houseBlock->Position->y = this->Position->y + this-> Size->cy - 20;
	step = 2;
}
void Magnet::MoveHouseBlock()
{
	if (houseBlock == nullptr) return;
	houseBlock->Position->x = this->Position->x + (this->Size->cx - houseBlock->Size->cx) / 2;
	houseBlock->Position->y = this->Position->y + this->Size->cy - 20;
}

void Magnet::Tick(int tick)
{
	if (magnetState == msMoving || delay > 0)
	{
		this->Position->x += step;
		if (this->Position->x < 20 || this->Position->x > (owner->Size->cx - this->Size->cx - 20)) step = -step;
	}

	if (magnetState == msShowing)
	{
		if (this->Position->y > 50) 
		{ 
			magnetState = msMoving;
			step = 1; // Зависит от уровня игры!!!
		}
		else
			this->Position->y += step;
	}
	if (magnetState == msUnshowing)
	{
		if (--delay < 0)
		{
			this->Position->y -= 2;
			this->Position->x -= ((this->Position->x - this->Size->cx / 2) - owner->Size->cx / 2) / 20;
			if (this->Position->y + Size->cy < 0) magnetState = msUnvisible;
		}
	}
	MoveHouseBlock();
}

std::shared_ptr<DropHouseBlock> Magnet::DropHouse()
{
	if (magnetState != msMoving) return nullptr;

	std::shared_ptr<DropHouseBlock> house( new DropHouseBlock(houseBlock, step));
	owner->DeleteObject(houseBlock);
	owner->AddObject(house);
	houseBlock.reset();
	houseBlock = nullptr;
	magnetState = msUnshowing;
	delay = 100;
	return house;
}
