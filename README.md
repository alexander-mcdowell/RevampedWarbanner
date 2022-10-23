# Revamped Warbanner

![Revamped Warbanner](https://github.com/alexander-mcdowell/RevampedWarbanner/blob/main/icon.png)

Changes the Warbanner item so that you would _actually_ seek it out as a worthwhile item that is fun to use. Disables warbanner placement on teleporter initiation and level up, but spawns on equipment usage. Reduces radius from 16m (+8m per banner) to 8m (+4m per banner).

There is a compatability issue with BetterUI's tooltips where it will always display the Warbanner radius as 16m (+8m/stack). I have raised this issue to BetterUI's developer. The actual radius of the Warbanner is given by the "New Radius" tag.

# Changelog
- 1.0.0
	- Initial Release
- 1.0.1
	- Added (partial) BetterUI compatability and configuration options for warbanner radius.
- 1.0.2
	- Fixed bug where Warbanners will spawn on equipment usage even if the player doesn't have a Warbanner... (sigh).
- 1.0.3
	- v1.0.2 appears to have fixed incompatability with TeammateRevival. The README has been edited to reflect this.
