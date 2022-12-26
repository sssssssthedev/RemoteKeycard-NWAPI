# RemoteKeycard-NWAPI
Original: https://github.com/iRebbok/RemoteKeycard
New: https://github.com/Parkeymon/RemoteKeycard (forked from https://github.com/SebasCapo/RemoteKeycard)

This was made for my server because the exiled version isn't ported yet, you can use it if you want but please know there are issues (also code is bad)

Issues:
- Pedestals, Generators and WarheadPanel doesn't work (there are no events for interacting with a generator or warheadpanel, idk how to make it work for pedestals)
- On lockers, the light flashes red then green, but it opens instantly, its because I have to update (RefreshOpenedSyncvar) after the state of the locker is set to open (If someone knows a better way to do this then make a pull request)
- Both lockers and doors audio have problems, the locker audio first does the no access sound then the granted beep, which still sounds fine, just a little weird, but the door audio just sounds the no access sound and then the door open sound but without the sound thats supposed to go after the door opens (If someone knows a way to fix this just make a pull request)
- There might be other bugs that I either forgot to include or I haven't tested for them, if you find any of them then make a issue.

