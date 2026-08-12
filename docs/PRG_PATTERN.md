# Post/Redirect/Get (PRG) Pattern

+ In MVC, `POST` actions that **modify *states*** should ***NOT* render views directly**;
+ The correct approach is to **redirect *after* the `POST`** to **avoid form resubmission on `F5`**.
 
  > `RedirectionToAction(nameof({index}))` follows this pattern.
