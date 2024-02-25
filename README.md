<h1>CSS_RespawnKiller</h1>

<section>
    <h2>Description</h2>
    <p>This is a plugin for Minigame (mg) Course made with <a href="https://docs.cssharp.dev/index.html">CounterStrikeSharp</a>. This plugin can auto-detect when the RespawnKill is active in CS2 course maps. This plugin is a kind of port of <a href="https://forums.alliedmods.net/showthread.php?p=2374048">Franc1sco Auto-Respawn for CS:GO</a>. This plugin needs test with multiple players, I've only tested in my LocalServer. If you test or use this leave me a feedback!</p>
</section>

<section>
    <h2>Dependencies</h2>
    <ul>
        <li><a href="https://cs2.poggu.me/metamod/installation/">MetaMod</a></li>
        <li><a href="https://github.com/roflmuffin/CounterStrikeSharp/releases">CounterStrikeSharp</a> (tested with v172)</li>
        <li><a href="https://github.com/DEAFPS/SharpTimer/tree/main">SharpTimer</a> (OPTIONAL for timer zone!)</li>
    </ul>
</section>

<section>
    <h2>Commands</h2>
    <table>
        <thead>
            <tr>
                <th>Command</th>
                <th>Argument</th>
                <th>Description</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td><code>css_autodetectrespawnkill</code></td>
                <td>&lt;0/1&gt;</td>
                <td>Enable or disable auto detection for respawn kill. If this is enabled the <code>css_setmaprespawntime</code> value will be setted to 0.</td>
            </tr>
            <tr>
                <td><code>css_setmaprespawntime</code></td>
                <td>&lt;seconds&gt;</td>
                <td>Set the respawn time in seconds for the map. 0 to ignore. If assign a time to respawn on the map, the <code>css_autodetectrespawnkill</code> will be turned off.</td>
            </tr>
        </tbody>
    </table>
</section>

<section>
    <h2>Config</h2>
    <table>
        <thead>
            <tr>
                <th>ConVar</th>
                <th>DefaultValue</th>
                <th>Description</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td><code>ChatPrefix</code></td>
                <td>"[RespawnKill]"</td>
                <td>String containing the prefix used to print chat and console commands.</td>
            </tr>
            <tr>
                <td><code>TimeDeadScreen</code></td>
                <td>0.5f</td>
                <td>Time that takes to the player to be respawned after he's dead.</td>
            </tr>
            <tr>
                <td><code>TimeBtwPlayerDeathsToDetectRespawnKill</code></td>
                <td>2.5f</td>
                <td>Works only when <code>css_autodetectrespawnkill</code> is turned on. It will check the time betweeen the player death, if the time is smaller than this variable, it will turn off respawn.</td>
            </tr>
            <tr>
                <td><code>LetPluginDecideForRoundEndConditions</code></td>
                <td>true</td>
                <td>Let the plugin decide when the round is going to end. It checks if all the players are dead every player death/disconnect/changeteam, and if they are, start a new round.</td>
            </tr>
            <tr>
                <td><code>DebugMessages</code></td>
                <td>false</td>
                <td>Display debug messages in console</td>
            </tr>
        </tbody>
    </table>
</section>

<section>
    <h2>Usage</h2>
    <p>All the maps that doesn't have a config start by default with <code>css_autodetectrespawnkill 1</code> and <code>css_setmaprespawntime 0</code>. If you change the values you will write to a .json in <code>/csgo/cfg/RespawnKiller/MapSettings/{MapName}.json</code> with your config value. These values will be executed every time this map is loaded.</p>
</section>

<section>
    <h2>To-Do</h2>
    <ul>
        <li>[x] RespawnKill Auto-Detection</li>
        <li>[x] Timer to disable Respawn</li>
        <li>[x] Create a .json list with mapnames and their info</li>
        <li>[x] Create a function that checks for the round end conditions when a player die (end the round if the respawn time has ended and everyone is dead *except the replaybot*)</li>
        <li>[ ] Test in a live server with bunch of ppl to search for problems</li>
        <li>[ ] Go to every course map in the workshop and make a .json for them</li>
    </ul>
</section>

<section>
    <h2>Contact</h2>
    <p>Join the <a href="https://discord.gg/eAZU3guKWU">CounterStrikeSharp Discord</a> and talk with me in my <a href="https://discord.com/channels/1160907911501991946/1211185159878082580">Plugin Post</a></p>
</section>
