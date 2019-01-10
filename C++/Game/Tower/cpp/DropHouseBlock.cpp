#include "../stdafx.h"
#include "../include/Magnet.h"
#include "../include/DropHouseBlock.h"

DropHouseBlock::DropHouseBlock(std::shared_ptr<HouseBlock> object, int step) : GameObject(object->GetOwner(), object->GetZOrder())
{
//	Count(this);
	resource = object->Resource;
	Position = object->Position;
	ZOrder = object->GetZOrder();
	this->step = abs(step);

	auto page = std::static_pointer_cast<PlayPage>(object->GetOwner());
	this->owner = page;
};

void DropHouseBlock::Init()
{
}

void DropHouseBlock::Tick(int tick)
{
	if (this->Position->y > owner->Size->cy) 
		step = 0; // Зависит от уровня игры!!!
	else
		this->Position->y += step;
}
