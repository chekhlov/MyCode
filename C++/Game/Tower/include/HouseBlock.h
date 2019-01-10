#pragma once

#include <string>
#include <vector>

#include "GameObject.h"
#include "ITickedObject.h"


class HouseBlock : public GameObject
{
private:
protected:
public:
	HouseBlock(std::shared_ptr<GameObject> owner, int ZOrder = 0);
	virtual ~HouseBlock();
};
