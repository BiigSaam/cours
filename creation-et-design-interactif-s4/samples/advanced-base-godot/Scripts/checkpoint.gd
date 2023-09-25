extends Area2D

func _on_body_entered(body):
	$CollisionShape2D.disabled = true
	body.data.current_spawn_position = position
