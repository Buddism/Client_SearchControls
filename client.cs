if(!isObject(OptRemapListSearchText))
{
	%text = new GuiTextEditCtrl(OptRemapListSearchText)
	{
		position = "6 370";
		command = "OptRemapList_Search();";
		//accelerator = "enter"; dont know how this works but it is fast enough realtime
		extent = "257 18";
	};

	OptControlsPane.add(%text);
}

function OptRemapListSearchText::onWake(%this, %b, %c, %d, %e, %f)
{
	//clear the search
	%this.setValue("");

	parent::onWake(%this, %b, %c, %d, %e, %f);
}


function OptRemapList_Search()
{
	%remapList = nameToID(OptRemapList);
	%string = OptRemapListSearchText.getValue();
	
	if(%string $= "")
	{
		OptRemapList.fillList();
		return;
	}

	%remapList.clear ();
	for(%i = 0; %i < $RemapCount; %i++)
	{
		%divName = $RemapDivision[%i];
		if (%divName !$= "")
		{
			%curDivName = %divName;
			%curDivNeedlePos = stripos(%divName, %string);
			if(%curDivNeedlePos != -1)
			{
				%displayDivision = true;

				//would use strReplace but its case-sensitive
				while(%curDivNeedlePos != -1)
				{
					%prefix   = getSubStr(%curDivName, 0, %curDivNeedlePos);
					%infix = getSubStr(%curDivName, %curDivNeedlePos, strlen(%string));
					%suffix   = getSubStr(%curDivName, %curDivNeedlePos + strlen(%string), 256);

					%curDivName =  %prefix @ "\c5/" @ %infix @ "/\c4" @ %suffix;

					//offset needs to include previously added characters (\c5, /,  /, \c4) \c# turn into 1 char
					%curDivNeedlePos = stripos(%curDivName, %string, %curDivNeedlePos + 4);
				}
			} else
				%displayDivision = false;

			%hasDisplayedDiv = false;
		}
		%displayString = buildFullMapString (%i);
		%needlePosition = stripos(%displayString, %string);

		if(%displayDivision || %needlePosition != -1)
		{
			if(!%hasDisplayedDiv)
			{
				if(%hasDisplayedOneDiv)
					%remapList.addRow (-1, "");

				%remapList.addRow (-1, "   \c4" @ %curDivName);
				%remapList.addRow (-1, "\c4------------------------------------------------------------------");

				%hasDisplayedDiv = true;
				%hasDisplayedOneDiv = true;
			}

			while(%needlePosition != -1)
			{
				%prefix   = getSubStr(%displayString, 0, %needlePosition);
				//ripped out the google for this bad boy: infix
				%infix = getSubStr(%displayString, %needlePosition, strlen(%string));

				%suffix   = getSubStr(%displayString, %needlePosition + strlen(%string), 256);

				%colorTypeHighlight = (%needlePosition < stripos(%displayString, "\t") ? "\c5" : "\c4");

				//colorstack: \cp is push, \co is pop
				%displayString =  %prefix @ "\cp" @ %colorTypeHighlight @ "/" @ %infix @ "/\co" @ %suffix;

				//offset needs to include previously added characters (\cp, \c#, /, /, \co)
				%needlePosition = stripos(%displayString, %string, %needlePosition + 5);
			}

			//					v - this argument is the most important for a GuiTextListCtrl
			%remapList.addRow (%i, %displayString);
		}
	}
}




