#pragma once
#ifndef _IGRAPHICENGINE_H_
#define _IGRAPHICENGINE_H_

#include "interface.h"
#include "IGraphicResource.h"
#include <memory>
#include <string>

interface IGraphicEngine : public IGraphicResource
{
	virtual ~IGraphicEngine() = 0 {};
	// Загрузка ресурса
	virtual std::shared_ptr<IGraphicResource> LoadResource(std::shared_ptr<IGraphicResource> owner, std::string fileName) = 0;

	virtual void Init(int sizeX, int sizeY) = 0;
	virtual void Free() = 0;
	virtual void PrepairRender() = 0;
	virtual void SwitchRender() = 0;
	virtual void FreeResource(std::shared_ptr<IGraphicResource> object) = 0;
	virtual void FreeResource(IGraphicResource *object) = 0;

};
#endif

