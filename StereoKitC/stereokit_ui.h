#pragma once

#include "stereokit.h"

namespace sk {

struct ui_settings_t {
	float padding;
	float gutter;
	float depth;
	float backplate_depth;
	float backplate_border;
};

SK_API void     ui_show_volumes(bool32_t show);
SK_API void     ui_settings (ui_settings_t settings);
SK_API void     ui_set_color(color128 color);

SK_API void     ui_layout_area (vec3 start, vec2 dimensions);
SK_API vec2     ui_area_remaining();
SK_API void     ui_nextline    ();
SK_API void     ui_sameline    ();
SK_API float    ui_line_height ();
SK_API void     ui_reserve_box (vec2 size);
SK_API void     ui_space       (float space);
SK_API uint64_t ui_push_id     (const char *id);
SK_API void     ui_pop_id      ();

SK_API bool32_t ui_is_interacting(handed_ hand);

SK_API void     ui_model_at       (model_t model, vec3 start, vec3 size, color128 color);
SK_API bool32_t ui_volume_at      (const char *id, bounds_t bounds);
SK_API bool32_t ui_button_at      (const char *text, vec3 window_relative_pos, vec2 size);
SK_API bool32_t ui_button_round_at(const char *text, sprite_t image, vec3 window_relative_pos, float diameter);
SK_API bool32_t ui_toggle_at      (const char *text, bool32_t &pressed, vec3 window_relative_pos, vec2 size);
SK_API bool32_t ui_hslider_at     (const char *id,   float &value, float min, float max, float step, vec3 window_relative_pos, vec2 size);

SK_API void     ui_label       (const char *text, bool32_t use_padding = true);
SK_API void     ui_image       (sprite_t image, vec2 size);
SK_API bool32_t ui_button      (const char *text);
SK_API bool32_t ui_button_round(const char *id, sprite_t image, float diameter = 0);
SK_API bool32_t ui_toggle      (const char *text, bool32_t &pressed);
//SK_API bool32_t ui_toggle_round(const char *text, bool32_t &pressed);
SK_API void     ui_model       (model_t model, vec2 ui_size, float model_scale);
SK_API bool32_t ui_input       (const char *id, char *buffer, int32_t buffer_size);
SK_API bool32_t ui_hslider     (const char *id, float &value, float min, float max, float step, float width = 0);
SK_API bool32_t ui_affordance_begin(const char *text, pose_t &movement, bounds_t handle, bool32_t draw);
SK_API void     ui_affordance_end  ();
SK_API void     ui_window_begin(const char *text, pose_t &pose, vec2 size = vec2{ 0,0 }, bool32_t show_header=true);
SK_API void     ui_window_end  ();

} // namespace sk