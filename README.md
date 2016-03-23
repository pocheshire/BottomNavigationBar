# BottomNavigationBar
<img src="https://raw.githubusercontent.com/roughike/BottomBar/master/scrolling_demo.gif" width="278" height="492" /> <img src="https://raw.githubusercontent.com/roughike/BottomBar/master/demo2-badge.gif" width="278" height="492" />  

**[How to contribute](https://github.com/pocheshire/BottomNavigationBar/blob/master/README.md#contributions)**

[Common problems and solutions](https://github.com/pocheshire/BottomNavigationBar/blob/master/README.md#common-problems-and-solutions)

## What?
[BottomBar](https://github.com/roughike/BottomBar) ported to C#

A custom view component that mimics the new [Material Design Bottom Navigation pattern](https://www.google.com/design/spec/components/bottom-navigation.html).

**(currently under development)**

## MinSDK version

The current minSDK version is **API level 11 (Honeycomb).**

## How?

BottomNavigationBar likes Fragments very much, but you can also handle your tab changes by yourself. You can add items by specifying an array of items or **by xml menu resources**.

#### Adding items from menu resource

**res/menu/bottombar_menu.xml:**

```xml
<menu xmlns:android="http://schemas.android.com/apk/res/android">
    <item
        android:id="@+id/bottomBarItemOne"
        android:icon="@drawable/ic_recents"
        android:title="Recents" />
        ...
</menu>
```

**MainActivity.cs**

```csharp
public class MainActivity : AppCompatActivity, BottomNavigationBar.Listeners.IOnMenuTabSelectedListener
{
    private BottomBar _bottomBar;
    
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        SetContentView(Resource.Layout.MainActivity);

        _bottomBar = BottomBar.Attach(this, bundle);
        _bottomBar.SetItemsFromMenu(Resource.Menu.bottombar_menu, this);
        _bottomBar.HideShadow();
        _bottomBar.UseDarkTheme(true);
        _bottomBar.SetTypeFace("Roboto-Regular.ttf");

        var badge = _bottomBar.MakeBadgeForTabAt(1, Color.ParseColor("#f02d4c"), 1);
        badge.AutoShowAfterUnSelection = true;
    }
    
    public void OnMenuItemSelected(int menuItemId)
    {
        
    }

    protected override void OnSaveInstanceState(Bundle outState)
    {
        base.OnSaveInstanceState(outState);

        // Necessary to restore the BottomBar's state, otherwise we would
        // lose the current tab on orientation change.
        _bottomBar.OnSaveInstanceState(outState);
    }
}
```

## Badges

You can easily add badges for showing an unread message count or new items / whatever you like.

```csharp
// Make a Badge for the first tab, with red background color and a value of "13".
BottomBarBadge unreadMessages = _bottomBar.MakeBadgeForTabAt(0, "#FF0000", 13);

// Control the badge's visibility
unreadMessages.Show();
unreadMessages.Hide();

// Change the displayed count for this badge.
unreadMessages.Count = 4;

// Change the show / hide animation duration.
unreadMessages.AnimationDuration = 200;

// If you want the badge be shown always after unselecting the tab that contains it.
unreadMessages.AutoShowAfterUnSelection = true;
```

## Customization

```csharp
// Disable the left bar on tablets and behave exactly the same on mobile and tablets instead.
_bottomBar.NoTabletGoodness();

// Use the dark theme. Ignored on mobile when there are more than three tabs.
_bottomBar.UseDarkTheme(true);

// Set the color for the active tab. Ignored on mobile when there are more than three tabs.
_bottomBar.SetActiveTabColor("#009688");

// Use custom text appearance in tab titles.
_bottomBar.SetTextAppearance(Resource.Style.MyTextAppearance);

// Use custom typeface that's located at the "/src/main/assets" directory. If using with
// custom text appearance, set the text appearance first.
_bottomBar.SetTypeFace("MyFont.ttf");
```

#### What about hiding it automatically on scroll?

**MainActivity.cs:**

```csharp
// Instead of attach(), use attachShy:
_bottomBar = BottomBar.AttachShy((CoordinatorLayout) FindViewById(Resource.Id.myCoordinator), 
    FindViewById(Resource.Id.myScrollingContent), savedInstanceState);
```

**activity_main.xml:**

```xml
<android.support.design.widget.CoordinatorLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:id="@+id/myCoordinator"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:fitsSystemWindows="true">

    <android.support.v4.widget.NestedScrollView
        android:id="@+id/myScrollingContent"
        android:layout_width="match_parent"
        android:layout_height="match_parent">

        <!-- Your loooong scrolling content here -->

    </android.support.v4.widget.NestedScrollView>

</android.support.design.widget.CoordinatorLayout>
```

#### Can it handle my Fragments and replace them automagically when a different tab is selected?

Yep yep yep! Just call ```SetFragmentItems()``` instead of ```SetItemsFromMenu()```:

```csharp
// If you use normal Fragments, just change the first argument to FragmentManager
_bottomBar.SetFragmentItems(SupportFragmentManager, Resource.Id.fragmentContainer,
    new BottomBarFragment(SampleFragment.NewInstance("Content for recents."), Resource.Drawable.ic_recents, "Recents"),
    new BottomBarFragment(SampleFragment.NewInstance("Content for favorites."), Resource.Drawable.ic_favorites, "Favorites"),
    new BottomBarFragment(SampleFragment.NewInstance("Content for nearby stuff."), Resource.Drawable.ic_nearby, "Nearby")
);
```

#### I hate Fragments and wanna do everything by myself!

That's alright, you can also do it the hard way if you're living on the edge.

```csharp
_bottomBar.SetItems(
        new BottomBarTab(Resource.Drawable.ic_recents, "Recents"),
        new BottomBarTab(Resource.Drawable.ic_favorites, "Favorites"),
        new BottomBarTab(Resource.Drawable.ic_nearby, "Nearby")
);

// Listen for tab changes
_bottomBar.SetOnItemSelectedListener(new OnTabSelectedListener());
```

For a working example, refer to [the sample app](https://github.com/roughike/BottomBar/tree/master/app/src/main).

## Common problems and solutions

#### Can I use it by XML?

No, but you can still put it anywhere in the View hierarchy. Just attach it to any View you want like this:

```csharp
_bottomBar.Attach(FindViewById(Resource.Id.myContent), savedInstanceState);
```

#### Why does the top of my content have sooooo much empty space?!

Probably because you're doing some next-level advanced Android stuff (such as using CoordinatorLayout and ```fitsSystemWindows="true"```) and the normal paddings for the content are too much. Add this right after calling ```Attach()```:

```csharp
_bottomBar.NoTopOffset();
```

#### I don't like the awesome transparent Navigation Bar!

You can disable it.

```csharp
_bottomBar.NoNavBarGoodness();
```

#### Why is it overlapping my Navigation Drawer?

All you need to do is instead of attaching the BottomBar to your Activity, attach it to the view that has your content. For example, if your fragments are in a ViewGroup that has the id ```fragmentContainer```, you would do something like this:

```csharp
_bottomBar.Attach(FindViewById(Resource.Id.fragmentContainer), savedInstanceState);
```

#### What about Tablets?

It works nicely with tablets straight out of the box. When the library detects that the user has a tablet, the BottomBar will become a "LeftBar", just like [in the Material Design Guidelines](https://material-design.storage.googleapis.com/publish/material_v_4/material_ext_publish/0B3321sZLoP_HSTd3UFY2aEp2ZDg/components_bottomnavigation_usage2.png).


#### What about the (insert thing that looks different than the specs here)?

Just do it!

## Apps using BottomNavigationBar

Send me a pull request with modified README.md to get a shoutout!

## Contributions

Feel free to create issues. 

I'm fixing issues _several hours_ every week. Your hard work could be for nothing, as I'm probably fixing / implementing the same problems that you are.

## License

```
BottomBar library for Android ported to C#
Copyright (c) 2016 Iiro Krankka (http://github.com/roughike).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```
