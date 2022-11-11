#pragma once
#include <string>

namespace FirstGameProject
{
	REGISTER_SCRIPT(character_script);
	class character_script : public theseus::script::entity_script
	{
	public:
		constexpr explicit character_script(theseus::game_entity::entity entity)
			: theseus::script::entity_script(entity) {}

		void update(float deltatime) override;
	};

}