#pragma once

#include "interface.h"
#include "../libs/sdl2/include/SDL.h"

interface IGraphicEngine;

interface IGraphicResource : public std::enable_shared_from_this<IGraphicResource>
{
	virtual ~IGraphicResource() = 0 {};
	virtual SIZE GetSize() = 0;
	
	virtual std::shared_ptr<IGraphicResource> GetOwner() = 0;
	virtual std::shared_ptr<IGraphicEngine> GetEngine() = 0;
	virtual void Free() = 0;
	virtual void Clear() = 0;

	// Загрузка ресурса
	virtual void LoadResource(std::string fileName) = 0;
	// Рисование объекта на родителе
	virtual void Draw(int x, int y, int cx = 0, int cy = 0) = 0;
	virtual void Draw(SDL_Rect *dstRect = NULL) = 0;

	// Рисование объекта на себе
	virtual void Draw(std::shared_ptr<IGraphicResource> object, SDL_Rect *dstRect = NULL, SDL_Rect *srcRect = NULL) = 0;
	virtual void Draw(std::shared_ptr<IGraphicResource> object, int x, int y, int cx = 0, int cy = 0) = 0;
};
