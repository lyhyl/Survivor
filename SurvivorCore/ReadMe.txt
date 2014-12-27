UIDisplayData:
    `Map & Heroes`  is member of SCGame
    pass them to C# Marshal by pointer
    C# will Marshal them properly
    BUT SurvivorCore DON'T promise `Map & Heroes` will NOT change their addres
    it means maybe it should be check if changed in `UIAdapter::Display` in every call

writeMtx:
    lock it when access `actionLog`
	BUT NO lock when calling `ApplyAction()`
	I think it is not necessary