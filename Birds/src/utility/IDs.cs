using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.utility
{
    public enum ID_ENTITY
    {
        DEFAULT,
        SHOOTER,
        PROJECTILE,
        COMPOSITE,
        CIRCULAR_COMPOSITE,
        SPIKE,
        LINK_COMPOSITE,
        TRIANGULAR_EQUAL_COMPOSITE,
        TRIANGULAR_90ANGLE_COMPOSITE,
        EMPTY_LINK,
        ENGINE,
        CLOUD,
        SUN,
    }
    public enum ID_CONTROLLER
    {
        PLAYER,
        CONTROLLER_DEFAULT,
        CHASER_AI,
        CIRCULAR_AI,
        INDECISIVE_AI,
        RANDOM_AI,
        BACKGROUND_SUN,
        FOREGROUND_CLOUD,
    }
    public enum ID_POSITION
    {
        POSITION_MIDDLE,
        POSITION_TOP_RIGHT,
        POSITION_NOT_BOUND,
    }
    public enum ID_SPRITE
    {
        HULL_RECTANGULAR,
        HULL_CIRCULAR,
        GUN,
        PROJECTILE,
        CLOUD,
        SUN,
        LINK_EMPTY,
        SPIKE,
        BUTTON_ENTITY,
        LINK_HULL,
        HULL_TRIANGULAR_EQUALLEGGED,
        HULL_TRIANGULAR_90DEGREES,
        ENGINE,
        BUTTON,
        BACKGROUND_GRAY,
        BACKGROUND_WHITE,
    }

    public enum ID_OTHER
    {
        UTILITY_ENTITY_BUTTON,

        TEAM_NEUTRAL_HOSTILE,
        TEAM_NEUTRAL_FRIENDLY,
        TEAM_PLAYER,
        TEAM_AI,
        FONT,
    }
}
