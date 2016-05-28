﻿using NServiceBus;
using StructureMap;

class Usage
{
    Usage(Configure configure)
    {
        #region StructureMap

        configure.StructureMapBuilder();

        #endregion
    }

    void Existing(Configure configure)
    {
        #region StructureMap_Existing

        var container = new Container(x => x.For<MyService>().Use(new MyService()));
        configure.StructureMapBuilder(container);

        #endregion
    }

    class MyService
    {
    }
}