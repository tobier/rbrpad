# RbrPad

Rumble implementation for **Richard Burns Rally** supporting XInput (Xbox-style) controllers.

## What is this?

Rumble supports is currently implemented as a proof-of-concept [SimHub](https://www.simhubdash.com/) plugin.
The end goal is to add rumble support as a standalone RBR plugin.

## Requirements

- SimHub (for PoC)
- Richard Burns Rally with NGP7, e.g. RallySimFans, with NGP UDP telemetry enabled
- An XInput-compatible (Xbox-style) controller

## Build & install

1. Build the project (`Tobier.RbrPad.csproj`, targets `net48`).
2. The post-build step copies the plugin DLL into the SimHub install folder.
3. Launch SimHub and enable the plugin (**RBR Gamepad Companion**) in its plugin list.
4. Configure from the left-hand menu.

