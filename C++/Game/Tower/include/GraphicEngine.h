#pragma once
#ifndef _GRAPHICENGINE_H_
#define _GRAPHICENGINE_H_

#include <windows.h>
#include "IGraphicEngine.h"
#include "GraphicResource.h"
#include "../libs/sdl2/include/SDL.h"
#include "../libs/sdl2/include/SDL_image.h"


// Класс обертка библиотеки SDL2
class GraphicEngine : public IGraphicEngine
{
private:
	static const long maxScreenSizeX = 800;
	static const long maxScreenSizeY = 600;
	static const long minScreenSizeX = 320;
	static const long minScreenSizeY = 200;
	virtual void LoadResource(std::string fileName) override {};

protected:
	bool isInit = false;
	SDL_Window *window = nullptr;
	SDL_Renderer *render = nullptr;
	SIZE size;
	GraphicEngine() {};

	virtual void Draw(IGraphicResource *object, SDL_Rect *dstRect = NULL, SDL_Rect *srcRect = NULL);
public:
	static const long GE_UseMaxSize = -1;

	GraphicEngine(int sizeX = GE_UseMaxSize, int sizeY = GE_UseMaxSize);
	virtual ~GraphicEngine();
	virtual SIZE GetMaximumScreenSize();
	virtual SIZE GetMinimumScreenSize();
	virtual SDL_Renderer* GetRender() ;

	ROProperty<SIZE, IGraphicResource, &GraphicEngine::GetSize> Size;

	// Реализация интерфейов IGraphicEngine
	virtual std::shared_ptr<IGraphicResource> LoadResource(std::shared_ptr<IGraphicResource> owner, std::string fileName) override;
	virtual void Init(int sizeX = GE_UseMaxSize, int sizeY = GE_UseMaxSize) override;
	virtual void Free() override;
	// для удаление ресурса из GraphicResource
	virtual void FreeResource(IGraphicResource *object) override;
	virtual void FreeResource(std::shared_ptr<IGraphicResource> object) override;
	virtual void PrepairRender() override;
	virtual void SwitchRender() override;

	virtual std::shared_ptr<IGraphicResource> GetOwner() override { return nullptr; };
	virtual std::shared_ptr<IGraphicEngine> GetEngine() override;
	virtual SIZE GetSize() override;
	virtual void Clear() override;
	// Рисовании на родителе
	virtual void Draw(int x, int y, int cx = 0, int cy = 0) override;
	virtual void Draw(SDL_Rect *dstRect = NULL) override;
	// Рисование на себе
	virtual void Draw(std::shared_ptr<IGraphicResource> object, SDL_Rect *dstRect = NULL, SDL_Rect *srcRect = NULL) override;
	virtual void Draw(std::shared_ptr<IGraphicResource> object, int x, int y, int cx = 0, int cy = 0) override;

	friend class GraphicResource;
	friend class TextObject;
};

#endif
