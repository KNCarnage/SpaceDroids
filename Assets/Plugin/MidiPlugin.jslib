mergeInto(LibraryManager.library, {
	InitMidi: function (sampleRate,bufferSize)
	{
		JSInitMidi(sampleRate,bufferSize);
	},

	FillFrontBuffer: function (frameCount,sampleRate,data,play)
	{
		var buffer = [];
		for (var i = 0; i < (frameCount*channels); i++)
		{
			buffer[i] = HEAPF32[(data >> 2) + i];
        }
		JSFillFrontBuffer (frameCount,sampleRate,buffer,play);
	},

	FillRearBuffer: function (frameCount,sampleRate,data,play)
	{
		var buffer = [];
		for (var i = 0; i < (frameCount*channels); i++)
		{
			buffer[i] = HEAPF32[(data >> 2) + i];
        }
		JSFillRearBuffer (frameCount,sampleRate,buffer,play);
	},
});