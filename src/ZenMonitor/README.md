## Compiling tailwind css is needed for styling

NOTE: Using the launch options inside `.vscode/`, everything will be done for you.
Hot reloads, tailwind compiling, everything. If you gotta compile manually, see below.

Run this inside `ZenMonitor/` to compile tailwind css manually.

```bash
npx @tailwindcss/cli -i ./css/input.css -o ./wwwroot/tailwind.css --watch
```

Then simply build and run it as you wish.
Whats important tho is that tailwind gets compiled,
or else the css rules cant be found by the application.