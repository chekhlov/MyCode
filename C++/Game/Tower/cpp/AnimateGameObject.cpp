#include "../stdafx.h"
#include "../include/AnimateGameObject.h"

AnimateGameObject::AnimateGameObject(std::shared_ptr<GameObject> owner, int ZOrder) : GameObject(owner, ZOrder)
{
	Count(this);
};

AnimateGameObject::~AnimateGameObject()
{
	 resources.clear();
};

std::shared_ptr<IGraphicResource> AnimateGameObject::GetFrame(int i)
{
	if (Count == 0)
		throw GameException("GameObject::GetSprite У объекта нет ресурсов");

	if (i > Count)
		throw GameException("GameObject::GetSprite Попытка получения несуществующего ресурса");

	return resources[i];
}

void AnimateGameObject::AddFrame(std::string fileName)
{
	std::shared_ptr<IGraphicResource>object(new GraphicResource(owner->resource));
	object->LoadResource(fileName);
	AddFrame(object);
}


void AnimateGameObject::AddFrame(std::shared_ptr<IGraphicResource> object)
{
	if (Count == 0) 
		SetResource(object);

	resources.push_back(object);
}

int AnimateGameObject::GetCount()
{
	return resources.size();
}

std::shared_ptr<IGraphicResource> AnimateGameObject::operator[](int i)
{
	return GetFrame(i);
}

void AnimateGameObject::SetFrame(std::shared_ptr<IGraphicResource> object)
{
	// Проверяем, что ресурс принадлежит текущему объекту.
	for (auto item : resources)
		if (item == object) SetResource(object);
}

void AnimateGameObject::SetFrame(int n)
{
	int cnt = Count;
	if (n >=0 && n < Count)
	this->resource = this->resources[n];
}

std::shared_ptr<IGraphicResource> AnimateGameObject::operator = (std::shared_ptr<IGraphicResource> &object)
{
	SetFrame(object);
	return object;
}
