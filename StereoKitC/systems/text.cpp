#include "text.h"

#include "../asset_types/material.h"
#include "../asset_types/assets.h"
#include "../systems/defaults.h"
#include "../hierarchy.h"
#include "../math.h"

#include <vector>
using namespace std;

#include <directxmath.h> // Matrix math functions and objects
using namespace DirectX;

namespace sk {

///////////////////////////////////////////

vector<_text_style_t> text_styles;
vector<text_buffer_t> text_buffers;

///////////////////////////////////////////

void text_buffer_ensure_capacity(text_buffer_t &buffer, size_t characters) {
	if (buffer.vert_count + characters * 4 <= buffer.vert_cap)
		return;

	buffer.vert_cap = buffer.vert_count + (int)characters * 4;
	buffer.verts    = (vert_t *)realloc(buffer.verts, sizeof(vert_t) * buffer.vert_cap);

	// regenerate indices
	vind_t  quads = (vind_t)(buffer.vert_cap / 4);
	vind_t *inds  = (vind_t *)malloc(quads * 6 * sizeof(vind_t));
	for (vind_t i = 0; i < quads; i++) {
		vind_t q = i * 4;
		vind_t c = i * 6;
		inds[c+0] = q+2;
		inds[c+1] = q+1;
		inds[c+2] = q;

		inds[c+3] = q+3;
		inds[c+4] = q+2;
		inds[c+5] = q;
	}
	mesh_set_inds(buffer.mesh, inds, quads * 6);
	free(inds);
}

///////////////////////////////////////////

text_style_t text_make_style(font_t font, float character_height, material_t material, color32 color) {
	uint32_t       id     = (uint32_t)(font->header.id << 16 | material->header.id);
	size_t         index  = 0;
	text_buffer_t *buffer = nullptr;
	
	// Find or make a buffer for this style
	for (size_t i = 0; i < text_buffers.size(); i++) {
		if (text_buffers[i].id == id) {
			buffer = &text_buffers[i];
			index  = i;
			break;
		}
	}
	if (buffer == nullptr) {
		text_buffers.push_back({});
		index  = text_buffers.size() - 1;
		buffer = &text_buffers[index];

		buffer->mesh     = mesh_create();
		buffer->id       = id;
		buffer->font     = font;
		buffer->material = material;
		assets_addref(font->header);
		assets_addref(material->header);

		material_set_texture     (material, "diffuse", font_get_tex(font));
		material_set_cull        (material, cull_none);
		material_set_transparency(material, transparency_blend);
	}

	// Create the style
	_text_style_t style;
	style.font            = font;
	style.buffer_index    = (uint32_t)index;
	style.color           = color;
	style.size            = character_height/font->character_height;
	style.line_spacing    = font->character_height * 0.25f;
	text_styles.push_back(style);

	return (text_style_t)(text_styles.size() - 1);
}

///////////////////////////////////////////

vec2 text_line_size(text_style_t style, const char *text) {
	font_t      font = text_styles[style].font;
	const char *curr = text;
	float       x    = 0;
	while (*curr != '\0' && *curr != '\n') {
		char         currch = *curr;
		font_char_t &ch     = font->characters[(int)currch];

		// Do spacing for whitespace characters
		switch (currch) {
		case '\t': x += font->characters[(int)' '].xadvance * 4; break;
		default:   x += ch.xadvance; break;
		}
		curr += 1;
	}
	return vec2{ x, font->character_height } * text_styles[style].size;
}

///////////////////////////////////////////

vec2 text_size(const char *text, text_style_t style) {
	font_t font = text_styles[style == -1 ? 0 : style].font;
	const char *curr = text;
	float x = 0;
	int   y = 1;
	float max_x = 0;
	while (*curr != '\0') {
		char currch = *curr;
		curr += 1;
		font_char_t &ch = font->characters[(int)currch];

		// Do spacing for whitespace characters
		switch (currch) {
		case '\t': x += font->characters[(int)' '].xadvance * 4; break;
		case '\n': if (x > max_x) max_x = x; x = 0; y += 1; break;
		default:   x += ch.xadvance; break;
		}
	}
	if (x > max_x) max_x = x;
	return vec2{ max_x, y * font->character_height + (y-1)*text_styles[style].line_spacing} * text_styles[style].size;
}

///////////////////////////////////////////

void text_add_at(const char* text, const matrix &transform, text_style_t style, text_align_ position, text_align_ align, float off_x, float off_y, float off_z) {
	XMMATRIX tr;
	if (hierarchy_enabled) {
		matrix_mul(transform, hierarchy_stack.back().transform, tr);
	} else {
		math_matrix_to_fast(transform, &tr);
	}

	text_style_t   styleId    = style == -1 ? 0 : style;
	_text_style_t &style_data = text_styles[styleId];
	text_buffer_t &buffer     = text_buffers[style_data.buffer_index];
	vec2           size       = text_size(text, styleId);
	float          ch_height  = style_data.font->character_height;

	// Resize array if we need more room for this text
	size_t length = strlen(text);
	text_buffer_ensure_capacity(buffer, length);
	
	vec3    normal  = matrix_mul_direction(tr, vec3_forward);
	const char*curr = text;
	vec2    line_sz = text_line_size(styleId, curr);
	float   start_x = off_x;
	float   y       = off_y - ch_height * style_data.size;
	if (position & text_align_y_center) y += (size.y / 2.f);
	if (position & text_align_y_bottom) y += size.y;
	if (position & text_align_x_center) start_x += size.x / 2.f;
	if (position & text_align_x_right)  start_x += size.x;
	float align_x = 0;
	if (align & text_align_x_center) align_x = ((size.x - line_sz.x) / 2.f);
	if (align & text_align_x_right)  align_x = (size.x - line_sz.x);
	float x = start_x - align_x;
	size_t  offset  = buffer.vert_count;

	/*hierarchy_set_enabled(false);
	float line_y = y + style_data.size * ch_height;
	line_add(
		matrix_mul_point(tr, { start_x, line_y, 0 }),
		matrix_mul_point(tr, { start_x-size.x, line_y, 0 }), {255,0,0,255}, 0.001f);

	line_add(
		matrix_mul_point(tr, { start_x, line_y-size.y, 0 }),
		matrix_mul_point(tr, { start_x-size.x, line_y-size.y, 0 }), {255,0,0,255}, 0.001f);
	hierarchy_set_enabled(true);*/

	while (*curr != '\0') {
		char currch = *curr;
		curr += 1;
		font_char_t &ch = style_data.font->characters[(int)currch];

		// Do spacing for whitespace characters
		switch (currch) {
		case '\t': x -= style_data.font->characters[(int)' '].xadvance * 4 * style_data.size; continue;
		case ' ':  x -= ch.xadvance * style_data.size; continue;
		case '\n': {
			line_sz = text_line_size(styleId, curr);
			align_x = 0;
			if (align & text_align_x_center) align_x = ((size.x - line_sz.x) / 2.f);
			if (align & text_align_x_right)  align_x = (size.x - line_sz.x);
			x = start_x - align_x;
			y -= style_data.size * (ch_height+style_data.line_spacing);
		} continue;
		default:break;
		}
		
		// Add a character quad
		buffer.verts[offset + 0] = { matrix_mul_point(tr, vec3{x - ch.x0 * style_data.size, y + ch.y0 * style_data.size, off_z}), normal, vec2{ch.u0, ch.v0}, style_data.color };
		buffer.verts[offset + 1] = { matrix_mul_point(tr, vec3{x - ch.x1 * style_data.size, y + ch.y0 * style_data.size, off_z}), normal, vec2{ch.u1, ch.v0}, style_data.color };
		buffer.verts[offset + 2] = { matrix_mul_point(tr, vec3{x - ch.x1 * style_data.size, y + ch.y1 * style_data.size, off_z}), normal, vec2{ch.u1, ch.v1}, style_data.color };
		buffer.verts[offset + 3] = { matrix_mul_point(tr, vec3{x - ch.x0 * style_data.size, y + ch.y1 * style_data.size, off_z}), normal, vec2{ch.u0, ch.v1}, style_data.color };

		buffer.vert_count += 4;
		x -= ch.xadvance * style_data.size;
		offset += 4;
	}
}

///////////////////////////////////////////

void text_update() {
	for (size_t i = 0; i < text_buffers.size(); i++) {
		text_buffer_t &buffer = text_buffers[i];
		if (buffer.vert_count <= 0)
			continue;

		mesh_set_verts(buffer.mesh, buffer.verts, buffer.vert_count, false);
		mesh_set_draw_inds(buffer.mesh, (buffer.vert_count / 4) * 6);

		render_add_mesh(buffer.mesh, buffer.material, matrix_identity);
		buffer.vert_count = 0;
	}
}

///////////////////////////////////////////

void text_shutdown() {
	for (size_t i = 0; i < text_buffers.size(); i++) {
		text_buffer_t &buffer = text_buffers[i];
		mesh_release(buffer.mesh);
		font_release(buffer.font);
		material_release(buffer.material);
		free(buffer.verts);
	}
	text_buffers.clear();
}

} // namespace sk