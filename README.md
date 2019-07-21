# csharpi, a C# / [Discord.Net](https://github.com/discord-net/Discord.Net) Tutorial That Works on a Pi!

This repository is intended for use with my blog series [C# Discord Bot on a Raspberry Pi](https://www.gngrninja.com/code/2019/3/10/c-discord-bot-on-raspberry-pi-simple-bot-with-config-file). 

## This branch's correlating blog post is [Command Handling](https://www.gngrninja.com/code/2019/4/1/c-discord-bot-command-handling).

Whether or not you use it to follow along with the posts, you can use it to learn how to make a C# Discord bot, and of course the Raspberry Pi part is completely optional.

## Using This Repository
There are a couple things you'll want to know when using this repo.

### Structure
This repo is setup with different branches representing the progression of features and blog posts.

* The first branch for getting started is [intro](https://github.com/gngrninja/csharpi/tree/intro), then the next is [command basics](https://github.com/gngrninja/csharpi/tree/02-command-basics), and so on.

### Building/Debugging
* When building/debugging this bot it will fail due to a missing config.json file when launched the first time. You'll want to put a config.json file with the following content in **/bin/Debug/netcoreapp2.x/**
```json
{
    "Token": "your_bot_token",
    "Prefix": ";"
}
```

For more details, please see my post on [getting started](https://www.gngrninja.com/code/2019/3/10/c-discord-bot-on-raspberry-pi-simple-bot-with-config-file).

Have fun!