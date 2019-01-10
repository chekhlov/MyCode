#pragma once

#include <string>
#include <vector>

#include "GameObject.h"
#include "../helpers/property.h"

// Базовый класс для всех объектов игры
class AnimateGameObject : public GameObject
{
private:
protected:
	std::vector<std::shared_ptr<IGraphicResource>> resources;
	virtual std::shared_ptr<IGraphicResource> GetFrame(int i);
	virtual int GetCount();
	virtual void SetFrame(std::shared_ptr<IGraphicResource> object);
	virtual void SetFrame(int n);
public:

	ROProperty<int, AnimateGameObject, &AnimateGameObject::GetCount> Count;
	
	AnimateGameObject(std::shared_ptr<GameObject> owner, int ZOrder = 0);
	virtual ~AnimateGameObject();
	virtual void AddFrame(std::shared_ptr<IGraphicResource> object);
	virtual void AddFrame(std::string fileName);
	virtual std::shared_ptr<IGraphicResource> operator[](int i);
	virtual std::shared_ptr<IGraphicResource> operator = (std::shared_ptr<IGraphicResource> &object);
};
