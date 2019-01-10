#pragma once
#ifndef _GAMEOBJECT_H_
#define _GAMEOBJECT_H_

#include <string>
#include <vector>

#include "GraphicResource.h"
#include "GraphicEngine.h"
#include "../helpers/property.h"
#include <list>

class TextObject;
class ButtonObject;

// Базовый класс для всех объектов игры
class GameObject : public std::enable_shared_from_this<GameObject>
{
private:
protected:
	std::shared_ptr<IGraphicResource> resource;
	std::shared_ptr<GameObject> owner;
	std::list<std::shared_ptr<GameObject>> objects;

	std::string name;
	int ZOrder = 0;
	GameObject();

	public:
	virtual SIZE GetSize();
	virtual int GetZOrder();
	virtual std::shared_ptr<GameObject> GetOwner();
	virtual std::shared_ptr<IGraphicResource> GetResource();
	virtual std::shared_ptr<IGraphicResource> SetResource(std::shared_ptr<IGraphicResource> const &object);

	Property<POINT> Position;
	ROProperty<SIZE, GameObject, &GameObject::GetSize> Size;
	RWProperty<std::shared_ptr<IGraphicResource>, GameObject, &GameObject::GetResource, &GameObject::SetResource> Resource;


	GameObject(std::shared_ptr<GameObject> owner, int ZOrder = 0);
	GameObject(std::shared_ptr<GameObject> owner, int cx, int cy, int ZOrder = 0);
	GameObject(std::shared_ptr<GameObject> owner, std::shared_ptr<IGraphicResource> resource, int ZOrder = 0);
	virtual ~GameObject();

	virtual void DeleteObjects();
	virtual void DeleteObject(std::shared_ptr<GameObject> object);
	virtual bool ExistsObject(std::shared_ptr<GameObject> object);

	virtual void AddObject(std::shared_ptr<GameObject> object, int ZOrder = 0);

	virtual std::shared_ptr<IGraphicResource> LoadResource(std::shared_ptr<IGraphicResource> owner, std::string fileName);
	virtual std::shared_ptr<IGraphicResource> LoadResource(std::string fileName);
	virtual std::shared_ptr<GameObject> AddObject(int ZOrder = 0);
	virtual std::shared_ptr<GameObject> AddObject(std::string resource, int ZOrder = 0);
	virtual std::shared_ptr<TextObject> AddTextObject(int ZOrder = -10);
	virtual std::shared_ptr<ButtonObject> AddButtonObject(std::string recource, int ZOrder = 0);

	virtual void Draw();

	virtual bool Visible();

	friend class GamePage;
	friend class ClickableObject;
	friend class AnimateGameObject;
	friend class TextObject;
};

#endif